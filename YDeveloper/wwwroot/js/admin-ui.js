
document.addEventListener('DOMContentLoaded', () => {

    // --- 1. Sidebar Toggle (Batch 2) ---
    const sidebarToggle = document.getElementById('sidebar-toggle');
    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', () => {
            document.body.classList.toggle('sidebar-collapsed');
            localStorage.setItem('sidebar-collapsed', document.body.classList.contains('sidebar-collapsed'));
        });
    }

    // Restore Sidebar State
    if (localStorage.getItem('sidebar-collapsed') === 'true') {
        document.body.classList.add('sidebar-collapsed');
    }

    // Mobile Sidebar
    const mobileToggle = document.getElementById('mobile-sidebar-toggle');
    const sidebar = document.querySelector('.admin-sidebar');

    // Create overlay if not exists
    let overlay = document.querySelector('.mobile-overlay');
    if (!overlay) {
        overlay = document.createElement('div');
        overlay.className = 'mobile-overlay';
        document.body.appendChild(overlay);
    }

    if (mobileToggle && sidebar) {
        mobileToggle.addEventListener('click', () => {
            sidebar.classList.add('show');
            overlay.classList.add('show');
        });

        // Close when clicking overlay
        overlay.addEventListener('click', () => {
            sidebar.classList.remove('show');
            overlay.classList.remove('show');
        });

        // Close on route change (if SPA) or link click
        sidebar.querySelectorAll('.nav-link').forEach(link => {
            link.addEventListener('click', () => {
                if (window.innerWidth < 768) {
                    sidebar.classList.remove('show');
                    overlay.classList.remove('show');
                }
            });
        });
    }

    // --- 2. Table Filters (Item 46 & 45) ---
    // Simple client-side search for any table with class 'table'
    const tables = document.querySelectorAll('.table');
    if (tables.length > 0) {
        const searchInput = document.querySelector('input[name="search"], .table-search');
        if (searchInput) {
            searchInput.addEventListener('keyup', function () {
                const value = this.value.toLowerCase();
                tables.forEach(table => {
                    const rows = table.querySelectorAll('tbody tr');
                    rows.forEach(row => {
                        const text = row.textContent.toLowerCase();
                        row.style.display = text.indexOf(value) > -1 ? '' : 'none';
                    });
                });
            });
        }
    }

    // --- 11. Profile Picture Preview (Item 18) ---
    const profileInput = document.querySelector('#profile-upload-input, .profile-upload-input');
    const profilePreview = document.querySelector('#profile-preview-img, .profile-preview-img');

    if (profileInput && profilePreview) {
        profileInput.addEventListener('change', function (e) {
            if (this.files && this.files[0]) {
                const reader = new FileReader();
                reader.onload = function (e) {
                    profilePreview.src = e.target.result;
                    profilePreview.classList.add('animate-pulse'); // Add effect
                    setTimeout(() => profilePreview.classList.remove('animate-pulse'), 1000);
                };
                reader.readAsDataURL(this.files[0]);
            }
        });
    }

    // --- 12. Slug Generation (Item 17) ---
    // Usage: <input id="Name"> -> <input id="Slug">
    const nameInput = document.getElementById('Name') || document.getElementById('Title');
    const slugInput = document.getElementById('Slug') || document.getElementById('Url');

    if (nameInput && slugInput) {
        nameInput.addEventListener('input', function () {
            if (!slugInput.getAttribute('readonly')) { // Only auto-gen if not locked
                slugInput.value = generateSlug(this.value);
            }
        });

        // Helper
        function generateSlug(text) {
            return text.toString().toLowerCase()
                .replace(/\s+/g, '-')           // Replace spaces with -
                .replace(/ğ/g, 'g').replace(/ü/g, 'u').replace(/ş/g, 's')
                .replace(/ı/g, 'i').replace(/ö/g, 'o').replace(/ç/g, 'c')
                .replace(/[^\w\-]+/g, '')       // Remove all non-word chars
                .replace(/\-\-+/g, '-')         // Replace multiple - with single -
                .replace(/^-+/, '')             // Trim - from start
                .replace(/-+$/, '');            // Trim - from end
        }
    }

    // --- 3. Drag & Drop File Areas (Batch 3) ---
    const fileInputs = document.querySelectorAll('input[type="file"]');
    fileInputs.forEach(input => {
        const wrapper = input.closest('.form-group') || input.parentElement;
        if (wrapper) {
            // Visual feedback
            const originalBorderCallback = () => wrapper.style.border = '';

            wrapper.addEventListener('dragover', (e) => {
                e.preventDefault();
                wrapper.classList.add('bg-light', 'border-primary', 'shadow-sm');
                wrapper.style.borderStyle = 'dashed';
            });
            wrapper.addEventListener('dragleave', () => {
                wrapper.classList.remove('bg-light', 'border-primary', 'shadow-sm');
                wrapper.style.borderStyle = '';
            });
            wrapper.addEventListener('drop', (e) => {
                e.preventDefault();
                wrapper.classList.remove('bg-light', 'border-primary', 'shadow-sm');
                wrapper.style.borderStyle = '';

                if (e.dataTransfer.files.length) {
                    const file = e.dataTransfer.files[0];
                    // Check file type/size if needed (Item 72/73)
                    input.files = e.dataTransfer.files;

                    // Update label or show file name
                    const label = wrapper.querySelector('label');
                    if (label) {
                        const originalText = label.getAttribute('data-original-text') || label.innerText;
                        if (!label.getAttribute('data-original-text')) label.setAttribute('data-original-text', originalText);

                        label.innerHTML = `<i class="bi bi-file-earmark-check text-success"></i> ${file.name}`;
                    }

                    // Trigger change event
                    const event = new Event('change');
                    input.dispatchEvent(event);
                }
            });
        }
    });

    // --- 10. Required Field Indicator (Item 61) ---
    // Automatically adds a red asterisk to labels of required inputs
    document.querySelectorAll('input[required], select[required], textarea[required]').forEach(input => {
        const id = input.id;
        if (id) {
            const label = document.querySelector(`label[for="${id}"]`);
            if (label && !label.innerHTML.includes('text-danger')) {
                label.innerHTML += ' <span class="text-danger">*</span>';
            }
        }
    });

    // --- 4. Dynamic Favicon (Badge) (Batch 3) ---
    // Simulate badge update
    window.setFaviconBadge = function (count) {
        // Needs a library or canvas manipulation. 
        // Simple title update for now
        if (count > 0) {
            document.title = `(${count}) ` + document.title.replace(/^\(\d+\)\s/, '');
        } else {
            document.title = document.title.replace(/^\(\d+\)\s/, '');
        }
    };

    // --- 5. Batch Select (Item 48) ---
    // Usage: <th class="check-all-th"><input type="checkbox" id="checkAll"></th>
    // Rows: <td><input type="checkbox" class="row-check"></td>
    const checkAll = document.getElementById('checkAll');
    if (checkAll) {
        checkAll.addEventListener('change', function () {
            const isChecked = this.checked;
            const checkboxes = document.querySelectorAll('.row-check');
            checkboxes.forEach(cb => {
                cb.checked = isChecked;
                // Optional: Highlight row
                const row = cb.closest('tr');
                if (row) {
                    if (isChecked) row.classList.add('table-active');
                    else row.classList.remove('table-active');
                }
            });
            updateBulkActions();
        });

        // Listen for individual changes to update Check All state and bulk actions
        document.addEventListener('change', function (e) {
            if (e.target.classList.contains('row-check')) {
                const total = document.querySelectorAll('.row-check').length;
                const checked = document.querySelectorAll('.row-check:checked').length;
                checkAll.checked = total === checked;

                // Toggle row highlight
                const row = e.target.closest('tr');
                if (row) {
                    if (e.target.checked) row.classList.add('table-active');
                    else row.classList.remove('table-active');
                }

                updateBulkActions();
            }
        });
    }

    function updateBulkActions() {
        const checkedCount = document.querySelectorAll('.row-check:checked').length;
        const bulkActionDiv = document.getElementById('bulk-actions');
        if (bulkActionDiv) {
            if (checkedCount > 0) {
                bulkActionDiv.classList.remove('d-none');
                const counter = bulkActionDiv.querySelector('.selected-count');
                if (counter) counter.textContent = checkedCount;
            } else {
                bulkActionDiv.classList.add('d-none');
            }
        }
    }

    // --- 6. Textarea Char Counter (Item 74) ---
    // Uses maxlength attribute. Example: <textarea maxlength="500"></textarea>
    document.querySelectorAll('textarea[maxlength]').forEach(textarea => {
        // Create counter element
        const wrapper = document.createElement('div');
        wrapper.className = 'position-relative';
        textarea.parentNode.insertBefore(wrapper, textarea);
        wrapper.appendChild(textarea);

        const counter = document.createElement('small');
        counter.className = 'position-absolute bottom-0 end-0 p-2 text-muted opacity-50 pe-none';
        counter.style.fontSize = '0.75rem';
        counter.textContent = `0 / ${textarea.maxLength}`;
        wrapper.appendChild(counter);

        textarea.addEventListener('input', function () {
            counter.textContent = `${this.value.length} / ${this.maxLength}`;
            if (this.value.length >= this.maxLength) {
                counter.classList.add('text-danger', 'fw-bold');
                counter.classList.remove('text-muted');
            } else {
                counter.classList.remove('text-danger', 'fw-bold');
                counter.classList.add('text-muted');
            }
        });
    });

    // --- 7. Scroll to Top (Item 83) ---
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

    // --- 8. Textarea Autosize (Item 75) ---
    const autoResizeTextareas = document.querySelectorAll('textarea.autosize');
    autoResizeTextareas.forEach(textarea => {
        textarea.setAttribute('style', 'height:' + (textarea.scrollHeight) + 'px;overflow-y:hidden;');
        textarea.addEventListener("input", OnInput, false);
    });

    function OnInput() {
        this.style.height = 0;
        this.style.height = (this.scrollHeight) + "px";
    }

    // --- 9. Global Toast Helper (Item 51) ---
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
});
