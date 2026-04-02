// Roadmap Item 65: Phone Input Mask
// Automatically formats phone numbers as (5XX) XXX-XXXX

document.addEventListener('DOMContentLoaded', function () {
    const phoneInputs = document.querySelectorAll('input[type="tel"], input[name*="Phone"], input[id*="Phone"]');

    phoneInputs.forEach(input => {
        input.addEventListener('input', function (e) {
            let x = e.target.value.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,4})/);
            e.target.value = !x[2] ? x[1] : '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '');
        });

        // Add placeholder
        if (!input.placeholder) {
            input.placeholder = "(5XX) XXX-XXXX";
        }
    });


    // Roadmap Item 66: Credit Card Input Mask (4-digit grouping)
    // Matches common credit card related names or IDs
    const ccInputs = document.querySelectorAll('.input-cc, input[id*="CardNumber"], input[name*="CardNumber"]');
    ccInputs.forEach(input => {
        input.addEventListener('input', function (e) {
            let value = e.target.value.replace(/\D/g, ''); // Remove non-digits
            let formattedValue = '';
            for (let i = 0; i < value.length; i++) {
                if (i > 0 && i % 4 === 0) {
                    formattedValue += ' ';
                }
                formattedValue += value[i];
            }
            // Max length usually 19 (16 digits + 3 spaces)
            e.target.value = formattedValue.substring(0, 19);
        });

        // Add placeholder
        if (!input.placeholder) {
            input.placeholder = "0000 0000 0000 0000";
        }
    });
});
