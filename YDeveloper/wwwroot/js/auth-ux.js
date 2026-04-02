/**
 * auth-ux.js
 * Enhances Login/Register pages with user-friendly features.
 * - Password Strength Meter
 * - Email Domain Suggestion
 * - Caps Lock Warning
 * - Auto-Trim inputs
 */

document.addEventListener('DOMContentLoaded', function () {
    const emailInput = document.querySelector('input[type="email"]');
    const passwordInput = document.querySelector('input[type="password"]');

    // 1. Auto-Trim Email
    if (emailInput) {
        emailInput.addEventListener('blur', function () {
            this.value = this.value.trim();
        });

        // 2. Email Domain Suggestion (Simple version)
        const domains = ['gmail.com', 'hotmail.com', 'outlook.com', 'yahoo.com', 'icloud.com'];
        const suggestionDiv = document.createElement('div');
        suggestionDiv.className = 'text-warning small mt-1 d-none fade-in';
        suggestionDiv.id = 'email-suggestion';
        emailInput.parentNode.appendChild(suggestionDiv);

        emailInput.addEventListener('input', function () {
            const val = this.value;
            const parts = val.split('@');
            if (parts.length === 2 && parts[1].length > 1) {
                const domain = parts[1];
                let match = null;

                // Simple Levenshtein-like check or common typo check
                domains.forEach(d => {
                    if (d !== domain && (isTypo(domain, d))) {
                        match = d;
                    }
                });

                if (match) {
                    suggestionDiv.innerHTML = `<i class="bi bi-lightbulb"></i> Bunu mu demek istediniz? <a href="#" class="fw-bold text-decoration-underline text-warning" id="apply-suggestion" data-val="${parts[0]}@${match}">${parts[0]}@${match}</a>`;
                    suggestionDiv.classList.remove('d-none');

                    document.getElementById('apply-suggestion').addEventListener('click', function (e) {
                        e.preventDefault();
                        emailInput.value = this.getAttribute('data-val');
                        suggestionDiv.classList.add('d-none');
                    });
                } else {
                    suggestionDiv.classList.add('d-none');
                }
            } else {
                suggestionDiv.classList.add('d-none');
            }
        });
    }

    function isTypo(input, target) {
        // Very basic typo detection: "gmil" vs "gmail"
        if (Math.abs(input.length - target.length) > 2) return false;

        let errors = 0;
        for (let i = 0; i < Math.min(input.length, target.length); i++) {
            if (input[i] !== target[i]) errors++;
        }
        return errors < 3 && errors > 0;
    }

    // 3. Password Strength
    if (passwordInput && !document.getElementById('password-strength-bar')) {
        // Create bar
        const barContainer = document.createElement('div');
        barContainer.className = 'progress mt-2 d-none';
        barContainer.style.height = '4px';
        barContainer.innerHTML = '<div class="progress-bar" role="progressbar" style="width: 0%"></div>';
        passwordInput.parentNode.appendChild(barContainer);

        const feedback = document.createElement('small');
        feedback.className = 'd-block mt-1 text-muted small';
        passwordInput.parentNode.appendChild(feedback);

        passwordInput.addEventListener('input', function () {
            const val = this.value;
            barContainer.classList.remove('d-none');
            const result = calculateStrength(val);

            const bar = barContainer.querySelector('.progress-bar');
            bar.style.width = result.score + '%';
            bar.className = 'progress-bar ' + result.class;
            feedback.innerText = result.message;
        });
    }

    function calculateStrength(password) {
        let score = 0;
        if (!password) return { score: 0, class: '', message: '' };

        if (password.length > 6) score += 20;
        if (password.length > 10) score += 20;
        if (/[A-Z]/.test(password)) score += 20;
        if (/[0-9]/.test(password)) score += 20;
        if (/[^A-Za-z0-9]/.test(password)) score += 20;

        let cls = 'bg-danger';
        let msg = 'Çok Zayıf';
        if (score > 40) { cls = 'bg-warning'; msg = 'Zayıf'; }
        if (score > 60) { cls = 'bg-info'; msg = 'Orta'; }
        if (score > 80) { cls = 'bg-success'; msg = 'Güçlü'; }

        return { score, class: cls, message: msg };
    }

    // 4. Caps Lock Warning
    if (passwordInput) {
        const capsWarning = document.createElement('div');
        capsWarning.className = 'position-absolute top-50 start-0 translate-middle-y ms-2 text-warning d-none';
        capsWarning.style.right = '40px';
        capsWarning.innerHTML = '<i class="bi bi-capslock-fill" title="Caps Lock Açık"></i>';
        // Needs proper positioning or simply append outside
        // Let's settle for a text warning below
        const capsDiv = document.createElement('div');
        capsDiv.className = 'text-warning small mt-1 d-none fade-in';
        capsDiv.innerHTML = '<i class="bi bi-capslock-fill"></i> Caps Lock Açık';
        passwordInput.parentNode.appendChild(capsDiv);

        passwordInput.addEventListener('keyup', function (e) {
            if (e.getModifierState("CapsLock")) {
                capsDiv.classList.remove('d-none');
            } else {
                capsDiv.classList.add('d-none');
            }
        });
    }

    // --- Item 16: Password Match Check ---
    // Targets 'Input.ConfirmPassword' or similar
    const confirmPassInput = document.querySelector('input[name*="ConfirmPassword"], input[id*="ConfirmPassword"]');
    if (passwordInput && confirmPassInput) {
        const matchFeedback = document.createElement('div');
        matchFeedback.className = 'mt-1 small fw-bold d-none fade-in';
        confirmPassInput.parentNode.appendChild(matchFeedback);

        const checkMatch = () => {
            if (confirmPassInput.value.length === 0) {
                matchFeedback.classList.add('d-none');
                confirmPassInput.classList.remove('is-invalid', 'is-valid');
                return;
            }

            matchFeedback.classList.remove('d-none');
            if (passwordInput.value === confirmPassInput.value) {
                matchFeedback.innerHTML = '<i class="bi bi-check-circle-fill text-success"></i> Şifreler Eşleşiyor';
                matchFeedback.className = 'mt-1 small fw-bold text-success fade-in';
                confirmPassInput.classList.add('is-valid');
                confirmPassInput.classList.remove('is-invalid');
            } else {
                matchFeedback.innerHTML = '<i class="bi bi-x-circle-fill text-danger"></i> Şifreler Eşleşmiyor';
                matchFeedback.className = 'mt-1 small fw-bold text-danger fade-in';
                confirmPassInput.classList.add('is-invalid');
                confirmPassInput.classList.remove('is-valid');
            }
        };

        passwordInput.addEventListener('input', checkMatch);
        confirmPassInput.addEventListener('input', checkMatch);
    }

    // 5. Show/Hide Password Toggle (Item 3)
    document.querySelectorAll('.password-toggle').forEach(icon => {
        icon.addEventListener('click', function (e) {
            e.preventDefault();
            // Find the input relative to the icon (assuming structure: wrapper > input + btn > icon)
            // Or wrapper > input ... btn > icon
            // Let's traverse up to .position-relative and find 'input'
            const wrapper = this.closest('.position-relative');
            if (wrapper) {
                const input = wrapper.querySelector('input');
                if (input) {
                    const type = input.getAttribute('type') === 'password' ? 'text' : 'password';
                    input.setAttribute('type', type);
                    // Toggle icon
                    this.classList.toggle('bi-eye');
                    this.classList.toggle('bi-eye-slash');
                }
            }
        });
    });

    // 6. Disable Submit Button on Post (Item 8)
    document.querySelectorAll('form').forEach(form => {
        form.addEventListener('submit', function () {
            if (!this.checkValidity()) return; // Don't disable if validation fails client-side

            // Wait a tick to ensure other handlers (reCaptcha) run if needed
            // But usually reCaptcha handles submission manually.
            // This is for standard forms.
            const btn = this.querySelector('button[type="submit"]');
            if (btn && !btn.disabled) {
                const originalText = btn.innerHTML;
                btn.style.width = getComputedStyle(btn).width; // Lock width to prevent jump
                btn.disabled = true;
                btn.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span> İşleniyor...';

                // Safety timeout (in case of connectivity issue or validation prevention we missed)
                setTimeout(() => {
                    btn.disabled = false;
                    btn.innerHTML = originalText;
                }, 15000);
            }
        });
    });

});
