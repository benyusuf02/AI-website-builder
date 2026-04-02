document.addEventListener('DOMContentLoaded', function () {

    // Back to Top Button Logic
    const backToTopBtn = document.getElementById('backToTop');
    if (backToTopBtn) {
        window.addEventListener('scroll', () => {
            if (window.scrollY > 300) {
                backToTopBtn.classList.remove('d-none');
                backToTopBtn.classList.add('d-block');
            } else {
                backToTopBtn.classList.remove('d-block');
                backToTopBtn.classList.add('d-none');
            }
        });
        backToTopBtn.addEventListener('click', (e) => {
            e.preventDefault();
            window.scrollTo({ top: 0, behavior: 'smooth' });
        });
    }

    // 2. Password Show/Hide Toggle
    document.querySelectorAll('.password-toggle').forEach(toggle => {
        toggle.addEventListener('click', function () {
            const input = this.previousElementSibling;
            if (input && input.type === 'password') {
                input.type = 'text';
                this.classList.remove('bi-eye');
                this.classList.add('bi-eye-slash');
            } else if (input) {
                input.type = 'password';
                this.classList.remove('bi-eye-slash');
                this.classList.add('bi-eye');
            }
        });
    });

    // 3. Button Loading Spinner (Auto-apply to forms)
    document.querySelectorAll('form').forEach(form => {
        form.addEventListener('submit', function () {
            if ($(this).valid && !$(this).valid()) return; // Verify jQuery validation success first

            const btn = this.querySelector('button[type="submit"]');
            if (btn && !btn.classList.contains('no-loading')) {
                const originalText = btn.innerHTML;
                btn.disabled = true;
                btn.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span> Lütfen bekleyin...';

                // Safety timeout to reset button if server doesn't respond (e.g. 10s)
                setTimeout(() => {
                    btn.disabled = false;
                    btn.innerHTML = originalText;
                }, 10000);
            }
        });
    });

    // 4. Input Mask (Simple Tel implementation)
    const phoneInputs = document.querySelectorAll('input[type="tel"]');
    phoneInputs.forEach(input => {
        input.addEventListener('input', function (e) {
            var x = e.target.value.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,4})/);
            e.target.value = !x[2] ? x[1] : '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '');
        });
    });
});
