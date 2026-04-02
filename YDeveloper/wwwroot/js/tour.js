document.addEventListener('DOMContentLoaded', () => {

    window.startTour = function () {
        if (typeof driver === 'undefined') return;

        const driverObj = driver.js.driver({
            showProgress: true,
            animate: true,
            steps: [
                { element: '.navbar-brand', popover: { title: 'Hoşgeldiniz! 👋', description: 'Yaptık.com paneline hoş geldiniz. Kısa bir tur yapalım mı?' } },
                { element: 'a[href*="Step1"]', popover: { title: 'Hızlı Başlangıç 🚀', description: 'Buraya tıklayarak hemen yeni bir web sitesi veya proje oluşturabilirsiniz.' } },
                { element: '#notifDropdown', popover: { title: 'Bildirimler 🔔', description: 'Önemli güncellemeleri ve mesajları buradan takip edebilirsiniz.' } },
                { element: '#userDropdown', popover: { title: 'Profiliniz 👤', description: 'Hesap ayarları, faturalar ve çıkış işlemi burada.' } },
                { element: '#bd-theme', popover: { title: 'Karanlık Mod 🌙', description: 'Gözlerinizi yormamak için temayı buradan değiştirebilirsiniz.' } },
                { element: '#restart-tour-btn', popover: { title: 'Turu Tekrarla ↺', description: 'Bu tura istediğiniz zaman buradan tekrar ulaşabilirsiniz.' } }
            ],
            onDestroy: (element, step, { config, state }) => {
                if (state.activeStepIndex === config.steps.length - 1) {
                    // Only celebrate if finished naturally
                    if (window.triggerConfetti) window.triggerConfetti();
                    if (typeof Swal !== 'undefined') {
                        Swal.fire({
                            title: 'Harika! 🎉',
                            text: 'Tur tamamlandı. Artık Yaptık.com dünyasına hazırsınız!',
                            icon: 'success',
                            timer: 2000,
                            showConfirmButton: false
                        });
                    }
                }
            }
        });

        driverObj.drive();
    };

    // Auto-start only if query param exists (e.g. after Registration)
    const urlParams = new URLSearchParams(window.location.search);
    if (urlParams.has('tour')) {
        // Clear param to prevent loop on refresh
        const newUrl = window.location.href.split('?')[0];
        window.history.replaceState({}, document.title, newUrl);

        setTimeout(() => window.startTour(), 1000); // Small delay for UI load
    }
});
