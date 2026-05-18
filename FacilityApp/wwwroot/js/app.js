// ── Dark mode ────────────────────────────────────────────────────────────
window.themeManager = {
    getTheme: () => localStorage.getItem('fa-theme') || 'light',
    setTheme: (theme) => {
        document.documentElement.setAttribute('data-bs-theme', theme);
        localStorage.setItem('fa-theme', theme);
    },
    init: () => {
        const saved = localStorage.getItem('fa-theme') || 'light';
        document.documentElement.setAttribute('data-bs-theme', saved);
        return saved;
    }
};

// ── Chart.js helpers ─────────────────────────────────────────────────────
window.chartManager = {
    _charts: {},

    renderTrendChart: (canvasId, labels, data) => {
        const canvas = document.getElementById(canvasId);
        if (!canvas) return;

        if (window.chartManager._charts[canvasId]) {
            window.chartManager._charts[canvasId].destroy();
        }

        const isDark = document.documentElement.getAttribute('data-bs-theme') === 'dark';
        const gridColor  = isDark ? 'rgba(255,255,255,0.08)' : 'rgba(0,0,0,0.06)';
        const labelColor = isDark ? '#adb5bd' : '#6c757d';

        const chart = new Chart(canvas, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Visitors',
                    data: data,
                    borderColor: '#1b6ec2',
                    backgroundColor: 'rgba(27,110,194,0.1)',
                    borderWidth: 2,
                    pointBackgroundColor: '#1b6ec2',
                    pointRadius: 3,
                    pointHoverRadius: 5,
                    fill: true,
                    tension: 0.4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        callbacks: {
                            label: ctx => ` ${ctx.parsed.y} visitor${ctx.parsed.y !== 1 ? 's' : ''}`
                        }
                    }
                },
                scales: {
                    x: {
                        grid: { color: gridColor },
                        ticks: { color: labelColor, maxTicksLimit: 10 }
                    },
                    y: {
                        grid: { color: gridColor },
                        ticks: { color: labelColor, precision: 0 },
                        beginAtZero: true
                    }
                }
            }
        });

        window.chartManager._charts[canvasId] = chart;
    },

    destroyChart: (canvasId) => {
        if (window.chartManager._charts[canvasId]) {
            window.chartManager._charts[canvasId].destroy();
            delete window.chartManager._charts[canvasId];
        }
    }
};

// ── SignalR notification toasts ──────────────────────────────────────────
window.notificationManager = {
    show: (title, message, type) => {
        const container = document.getElementById('toast-container');
        if (!container) return;

        const id = 'toast-' + Date.now();
        const icon = type === 'success'
            ? '<i class="bi bi-person-check-fill text-success me-2"></i>'
            : '<i class="bi bi-bell-fill text-primary me-2"></i>';

        container.insertAdjacentHTML('beforeend', `
          <div id="${id}" class="toast align-items-center border-0 shadow" role="alert">
            <div class="d-flex">
              <div class="toast-body d-flex align-items-center gap-2">
                ${icon}
                <div>
                  <div class="fw-semibold small">${title}</div>
                  <div class="text-muted small">${message}</div>
                </div>
              </div>
              <button type="button" class="btn-close me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
          </div>`);

        const toastEl = document.getElementById(id);
        const toast   = new bootstrap.Toast(toastEl, { delay: 6000 });
        toast.show();
        toastEl.addEventListener('hidden.bs.toast', () => toastEl.remove());
    }
};

// ── Print badge ───────────────────────────────────────────────────────────
window.printBadge = () => window.print();
