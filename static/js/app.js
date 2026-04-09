(() => {
    const shell = document.querySelector('.tab-shell');
    if (!shell) {
        return;
    }

    const defaultTab = shell.dataset.defaultTab || 'quote';
    const links = Array.from(shell.querySelectorAll('.tab-link'));
    const panels = Array.from(shell.querySelectorAll('.tab-panel'));

    const activateTab = (name) => {
        links.forEach((link) => {
            link.classList.toggle('is-active', link.dataset.tab === name);
        });
        panels.forEach((panel) => {
            panel.classList.toggle('is-active', panel.dataset.tab === name);
        });
    };

    links.forEach((link) => {
        link.addEventListener('click', () => activateTab(link.dataset.tab));
    });

    activateTab(defaultTab);

    const table = document.getElementById('quote-lines-table');
    const template = document.getElementById('quote-line-template');
    const addRowButton = document.querySelector('[data-add-row]');

    if (table && template && addRowButton) {
        const tbody = table.querySelector('tbody');
        addRowButton.addEventListener('click', () => {
            tbody.appendChild(template.content.cloneNode(true));
        });

        table.addEventListener('click', (event) => {
            const target = event.target;
            if (!(target instanceof HTMLElement)) {
                return;
            }
            if (!target.matches('[data-remove-row]')) {
                return;
            }
            const row = target.closest('tr');
            if (!row) {
                return;
            }
            if (tbody.querySelectorAll('tr').length <= 1) {
                row.querySelectorAll('input').forEach((input) => {
                    input.value = '';
                });
                return;
            }
            row.remove();
        });
    }
})();
