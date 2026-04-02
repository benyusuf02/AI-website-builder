// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Lazy Loading
document.addEventListener("DOMContentLoaded", function () {
    var lazyImages = [].slice.call(document.querySelectorAll("img.lazy"));
    if ("IntersectionObserver" in window) {
        let lazyImageObserver = new IntersectionObserver(function (entries, observer) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    let lazyImage = entry.target;
                    lazyImage.src = lazyImage.dataset.src;
                    lazyImage.classList.remove("lazy");
                    lazyImageObserver.unobserve(lazyImage);
                }
            });
        });
        lazyImages.forEach(function (lazyImage) {
            lazyImageObserver.observe(lazyImage);
        });
    }

    // Auto-add loading="lazy" to all content images if not present
    document.querySelectorAll('img:not([loading])').forEach(img => {
        img.setAttribute('loading', 'lazy');
    });

    // --- Item 5: Auto Trim Inputs ---
    // Automatically trims whitespace from start/end of text inputs on blur
    document.addEventListener('focusout', function (e) {
        if (e.target.tagName === 'INPUT' && e.target.type === 'text') {
            e.target.value = e.target.value.trim();
        }
    });

    // --- Item 18: Profile Picture Preview ---
    // Works with Inputs having class 'profile-upload-input' and Img having class 'profile-preview-img' or specific ID structure
    const profileInputs = document.querySelectorAll('#Input_ProfilePicture, .profile-upload-input');
    profileInputs.forEach(input => {
        input.addEventListener('change', function (e) {
            if (this.files && this.files[0]) {
                const reader = new FileReader();
                reader.onload = function (e) {
                    // Try to find the image in the container
                    const container = input.closest('form') || document.body;
                    let img = container.querySelector('img.rounded-circle');

                    if (img) {
                        img.src = e.target.result;
                        img.classList.add('animate-pulse');
                        setTimeout(() => img.classList.remove('animate-pulse'), 1000);
                    } else {
                        // Check for placeholder div
                        const placeholder = container.querySelector('.rounded-circle.bg-light');
                        if (placeholder) {
                            const newImg = document.createElement('img');
                            newImg.src = e.target.result;
                            newImg.alt = "Profile Picture";
                            newImg.className = "rounded-circle shadow-sm animate-pulse";
                            newImg.style.width = "150px";
                            newImg.style.height = "150px";
                            newImg.style.objectFit = "cover";
                            placeholder.parentNode.replaceChild(newImg, placeholder);
                        }
                    }
                };
                reader.readAsDataURL(this.files[0]);
            }
        });
    });

    // --- Item 17: Slug Generation ---
    // Usage: <input id="Name"> -> <input id="Slug">
    const nameInput = document.getElementById('Name') || document.getElementById('Title');
    const slugInput = document.getElementById('Slug') || document.getElementById('Url');

    if (nameInput && slugInput) {
        nameInput.addEventListener('input', function () {
            if (!slugInput.getAttribute('readonly')) {
                slugInput.value = generateSlug(this.value);
            }
        });
    }

    function generateSlug(text) {
        return text.toString().toLowerCase()
            .replace(/\s+/g, '-')
            .replace(/ğ/g, 'g').replace(/ü/g, 'u').replace(/ş/g, 's')
            .replace(/ı/g, 'i').replace(/ö/g, 'o').replace(/ç/g, 'c')
            .replace(/[^\w\-]+/g, '')
            .replace(/\-\-+/g, '-')
            .replace(/^-+/, '')
            .replace(/-+$/, '');
    }

    // --- Item 83: Scroll to Top ---
    const backToTopBtn = document.getElementById('backToTop');
    if (backToTopBtn) {
        window.addEventListener('scroll', () => {
            if (window.scrollY > 300) {
                backToTopBtn.classList.remove('d-none');
                backToTopBtn.classList.add('animate-fade-in');
            } else {
                backToTopBtn.classList.add('d-none');
                backToTopBtn.classList.remove('animate-fade-in');
            }
        });

        backToTopBtn.addEventListener('click', () => {
            window.scrollTo({ top: 0, behavior: 'smooth' });
        });
    }

    // --- Item 75: Textarea Autosize ---
    const autoResizeTextareas = document.querySelectorAll('textarea.autosize');
    autoResizeTextareas.forEach(textarea => {
        textarea.setAttribute('style', 'height:' + (textarea.scrollHeight) + 'px;overflow-y:hidden;');
        textarea.addEventListener("input", OnInput, false);
    });

    function OnInput() {
        this.style.height = 0;
        this.style.height = (this.scrollHeight) + "px";
    }

    // --- Item 51: Global Toast Helper ---
    // --- Item 51: Global Toast Helper ---
    window.showToast = function (icon, title) {
        if (typeof Swal !== 'undefined') {
            const Toast = Swal.mixin({
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true,
                didOpen: (toast) => {
                    toast.addEventListener('mouseenter', Swal.stopTimer)
                    toast.addEventListener('mouseleave', Swal.resumeTimer)
                }
            });
            Toast.fire({ icon: icon, title: title });
        }
    };

    // --- Item 31: Confetti Helper ---
    window.triggerConfetti = function () {
        if (typeof confetti !== 'undefined') {
            confetti({
                particleCount: 100,
                spread: 70,
                origin: { y: 0.6 }
            });
        }
    };

    // --- Item 27: Exit Intent Popup (REMOVED) ---
    // User feedback: Too annoying.
});
