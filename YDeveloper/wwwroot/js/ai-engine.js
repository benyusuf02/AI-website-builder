/* AI Command Engine Frontend Client */

document.addEventListener('DOMContentLoaded', () => {
    const input = document.getElementById('heroPrompt');
    const button = document.querySelector('.hero-search-box a'); // The "Oluştur" button

    if (!input || !button) return;

    // Override default button behavior
    button.addEventListener('click', async (e) => {
        const prompt = input.value;

        // If input is empty, let the link work (navigate to Onboarding)
        if (!prompt) return;

        e.preventDefault(); // Only hijack if we have a command to process

        // Visual feedback
        const originalText = button.innerHTML;
        button.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> İşleniyor...';
        button.classList.add('disabled');

        try {
            const response = await fetch('/api/ai/command', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ prompt: prompt })
            });

            if (!response.ok) throw new Error('Network error');

            const data = await response.json();
            handleAiResponse(data);

        } catch (err) {
            console.error(err);
            Swal.fire({
                icon: 'error',
                title: 'Hata',
                text: 'Komut işlenirken bir sorun oluştu.'
            });
        } finally {
            button.innerHTML = originalText;
            button.classList.remove('disabled');
        }
    });

    // Allow Enter key
    input.addEventListener('keypress', (e) => {
        if (e.key === 'Enter') {
            button.click();
        }
    });
});

function handleAiResponse(data) {
    if (!data.success) {
        Swal.fire({
            icon: 'info',
            title: 'Anlaşılmadı',
            text: data.message || 'Ne yapmak istediğinizi tam anlayamadım.'
        });
        return;
    }

    // Show AI Message Toast
    if (data.message) {
        const Toast = Swal.mixin({
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true
        });
        Toast.fire({
            icon: 'success',
            title: data.message
        });
    }

    // Execute Actions
    switch (data.actionType) {
        case 1: // Redirect
            if (data.payload && data.payload.url) {
                setTimeout(() => {
                    window.location.href = data.payload.url;
                }, 1000); // Wait for toast
            }
            break;

        case 2: // StyleUpdate
            if (data.payload && data.payload.styles) {
                const selector = data.payload.selector || ':root';
                const el = document.querySelector(selector);
                if (el) {
                    for (const [key, value] of Object.entries(data.payload.styles)) {
                        el.style.setProperty(key, value);
                    }
                }
            }
            break;

        case 3: // DomManipulation
            // Implement later
            break;

        case 4: // Toast Only
            // Already handled above
            break;
    }
}
