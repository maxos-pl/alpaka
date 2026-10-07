// zoo-alpaca.js - Клиентская логика для страницы Альпаки

document.addEventListener('DOMContentLoaded', () => {
    initLiveCam();
    initDonationForm();
    initDiaryControls();
});

// Получение токена из cookies или localStorage
function getAuthToken() {
    const cookies = document.cookie.split(';');
    for (let c of cookies) {
        c = c.trim();
        if (c.startsWith('ZooAuthToken=')) {
            return c.substring('ZooAuthToken='.length);
        }
    }
    return localStorage.getItem('zoo_jwt_token') || '';
}

// 1. Веб-камера (переключение и часы)
function initLiveCam() {
    const clockEl = document.getElementById('webcamClock');
    if (clockEl) {
        setInterval(() => {
            const now = new Date();
            clockEl.textContent = now.toLocaleTimeString('ru-RU') + ' (МСК)';
        }, 1000);
    }

    const camButtons = document.querySelectorAll('.cam-switch-btn');
    const videoPlayer = document.getElementById('alpacaLivePlayer');
    const camLocationText = document.getElementById('camLocationText');

    camButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            const camName = btn.getAttribute('data-cam-name');
            const streamSrc = btn.getAttribute('data-stream-src');

            if (camLocationText) camLocationText.textContent = camName;
            if (videoPlayer && streamSrc) {
                videoPlayer.src = streamSrc;
                videoPlayer.play().catch(e => console.log('Autoplay prevented:', e));
            }
        });
    });
}

// 2. Отправка доната
function initDonationForm() {
    const form = document.getElementById('donationForm');
    if (!form) return;

    form.addEventListener('submit', async (e) => {
        e.preventDefault();

        const submitBtn = form.querySelector('button[type="submit"]');
        const alertBox = document.getElementById('donationAlert');
        const amountInput = document.getElementById('donationAmountInput');
        const nameInput = document.getElementById('donorNameInput');
        const targetSelect = document.getElementById('donationTargetSelect');
        const messageInput = document.getElementById('donorMessageInput');

        const amount = parseFloat(amountInput.value);
        if (isNaN(amount) || amount < 10) {
            showAlert(alertBox, 'Пожалуйста, введите сумму пожертвования от 10 ₽');
            return;
        }

        const payload = {
            animalSlug: 'alpaca',
            donorName: nameInput.value.trim() || 'Анонимный друг зоопарка',
            amount: amount,
            target: targetSelect.value,
            message: messageInput.value.trim()
        };

        try {
            submitBtn.disabled = true;
            submitBtn.textContent = 'Отправка...';

            const response = await fetch('/api/donations', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + getAuthToken()
                },
                body: JSON.stringify(payload)
            });

            const data = await response.json();

            if (response.ok && data.success) {
                showAlert(alertBox, `Благодарим! ${data.message}`);
                form.reset();

                if (data.summary) {
                    const totalEl = document.getElementById('totalRaisedText');
                    const donorsCountEl = document.getElementById('donorsCountText');
                    if (totalEl) totalEl.textContent = data.summary.totalRaised.toLocaleString('ru-RU') + ' ₽';
                    if (donorsCountEl) donorsCountEl.textContent = data.summary.donorsCount;
                }

                if (data.donation) {
                    prependRecentDonation(data.donation);
                }
            } else {
                showAlert(alertBox, data.message || 'Ошибка при отправке доната');
            }
        } catch (err) {
            showAlert(alertBox, 'Ошибка сети при отправке доната');
        } finally {
            submitBtn.disabled = false;
            submitBtn.textContent = 'Отправить донат';
        }
    });
}

function prependRecentDonation(d) {
    const tbody = document.getElementById('recentDonationsList');
    if (!tbody) return;

    const tr = document.createElement('tr');
    tr.innerHTML = `
        <td>Только что</td>
        <td>${escapeHtml(d.donorName)}</td>
        <td><strong>+${d.amount.toLocaleString('ru-RU')} ₽</strong></td>
        <td>${escapeHtml(d.target)}</td>
        <td>${d.message ? escapeHtml(d.message) : '-'}</td>
    `;
    tbody.insertBefore(tr, tbody.firstChild);
}

// 3. Управление дневником наблюдений
function initDiaryControls() {
    const filterButtons = document.querySelectorAll('.diary-filter-btn');
    const searchInput = document.getElementById('diarySearchInput');
    const addEntryForm = document.getElementById('addDiaryForm');

    filterButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            const cat = btn.getAttribute('data-category');
            loadDiaryEntries(cat, searchInput ? searchInput.value : '');
        });
    });

    if (searchInput) {
        let debounceTimeout;
        searchInput.addEventListener('input', () => {
            clearTimeout(debounceTimeout);
            debounceTimeout = setTimeout(() => {
                loadDiaryEntries('Все', searchInput.value);
            }, 300);
        });
    }

    if (addEntryForm) {
        addEntryForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            await submitNewDiaryEntry(addEntryForm);
        });
    }

    const exportJsonBtn = document.getElementById('exportDiaryJsonBtn');
    if (exportJsonBtn) {
        exportJsonBtn.addEventListener('click', () => exportDiary('json'));
    }
    const exportCsvBtn = document.getElementById('exportDiaryCsvBtn');
    if (exportCsvBtn) {
        exportCsvBtn.addEventListener('click', () => exportDiary('csv'));
    }
}

async function loadDiaryEntries(category = 'Все', search = '') {
    const tbody = document.getElementById('diaryTableBody');
    if (!tbody) return;

    let url = `/api/animals/alpaca/diary?`;
    if (category && category !== 'Все') url += `category=${encodeURIComponent(category)}&`;
    if (search) url += `search=${encodeURIComponent(search)}`;

    try {
        tbody.innerHTML = `<tr><td colspan="6">Загрузка записей дневника...</td></tr>`;
        const res = await fetch(url);
        const data = await res.json();

        if (!data || data.length === 0) {
            tbody.innerHTML = `<tr><td colspan="6">Записи не найдены.</td></tr>`;
            return;
        }

        renderDiaryTable(data, tbody);
    } catch (e) {
        tbody.innerHTML = `<tr><td colspan="6" style="color: red;">Ошибка загрузки дневника</td></tr>`;
    }
}

function renderDiaryTable(entries, tbody) {
    const isEmployee = document.body.getAttribute('data-user-role') === 'Employee' || document.body.getAttribute('data-user-role') === 'Admin';

    tbody.innerHTML = entries.map(entry => {
        const dateStr = new Date(entry.date).toLocaleString('ru-RU');

        return `
            <tr id="diary-row-${entry.id}">
                <td>${dateStr}</td>
                <td><strong>${escapeHtml(entry.category)}</strong></td>
                <td>
                    <strong>${escapeHtml(entry.title)}</strong>
                    <p style="margin: 4px 0 0 0;"><small>${escapeHtml(entry.description)}</small></p>
                </td>
                <td>
                    <small>
                        Статус: ${escapeHtml(entry.healthStatus || 'В норме')}<br />
                        ${entry.weightKg ? `Вес: ${entry.weightKg} кг<br />` : ''}
                        ${entry.temperatureC ? `Темп: ${entry.temperatureC} °C<br />` : ''}
                        ${entry.dietDetails ? `Рацион: ${escapeHtml(entry.dietDetails)}` : ''}
                    </small>
                </td>
                <td>${escapeHtml(entry.authorName || 'Сотрудник')}</td>
                ${isEmployee ? `
                    <td>
                        <button type="button" onclick="deleteDiaryEntry(${entry.id})" style="color: red;">Удалить</button>
                    </td>
                ` : ''}
            </tr>
        `;
    }).join('');
}

async function submitNewDiaryEntry(form) {
    const alertBox = document.getElementById('addDiaryAlert');
    const submitBtn = form.querySelector('button[type="submit"]');

    const payload = {
        category: form.Category.value,
        title: form.Title.value.trim(),
        description: form.Description.value.trim(),
        healthStatus: form.HealthStatus.value,
        dietDetails: form.DietDetails.value.trim(),
        weightKg: form.WeightKg.value ? parseFloat(form.WeightKg.value) : null,
        temperatureC: form.TemperatureC.value ? parseFloat(form.TemperatureC.value) : null,
        date: form.Date.value ? new Date(form.Date.value).toISOString() : new Date().toISOString()
    };

    try {
        submitBtn.disabled = true;
        submitBtn.textContent = 'Сохранение...';

        const token = getAuthToken();
        const headers = { 'Content-Type': 'application/json' };
        if (token) {
            headers['Authorization'] = 'Bearer ' + token;
        }

        const res = await fetch('/api/animals/alpaca/diary', {
            method: 'POST',
            headers: headers,
            body: JSON.stringify(payload)
        });

        if (res.ok) {
            form.reset();
            loadDiaryEntries();
            alert('Запись успешно сохранена в дневник!');
        } else if (res.status === 401 || res.status === 403) {
            showAlert(alertBox, 'Недостаточно прав. Требуется авторизация работника зоопарка.');
        } else {
            const err = await res.json();
            showAlert(alertBox, err.message || 'Ошибка сохранения');
        }
    } catch (e) {
        showAlert(alertBox, 'Ошибка при обращении к API');
    } finally {
        submitBtn.disabled = false;
        submitBtn.textContent = 'Сохранить запись в дневник';
    }
}

async function deleteDiaryEntry(id) {
    if (!confirm('Удалить эту запись из дневника?')) return;

    try {
        const token = getAuthToken();
        const headers = {};
        if (token) headers['Authorization'] = 'Bearer ' + token;

        const res = await fetch(`/api/animals/alpaca/diary/${id}`, {
            method: 'DELETE',
            headers: headers
        });

        if (res.ok) {
            const el = document.getElementById(`diary-row-${id}`);
            if (el) el.remove();
        } else {
            alert('Ошибка удаления записи (проверьте права)');
        }
    } catch (e) {
        alert('Ошибка при удалении');
    }
}

async function exportDiary(format) {
    try {
        const res = await fetch('/api/animals/alpaca/diary');
        const data = await res.json();

        if (format === 'json') {
            const blob = new Blob([JSON.stringify(data, null, 2)], { type: 'application/json' });
            downloadBlob(blob, 'alpaca-diary.json');
        } else if (format === 'csv') {
            let csv = '\uFEFFId,Date,Category,Title,Description,HealthStatus,WeightKg,TemperatureC,Author\n';
            data.forEach(item => {
                csv += `"${item.id}","${item.date}","${escapeCsv(item.category)}","${escapeCsv(item.title)}","${escapeCsv(item.description)}","${escapeCsv(item.healthStatus)}","${item.weightKg || ''}","${item.temperatureC || ''}","${escapeCsv(item.authorName)}"\n`;
            });
            const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
            downloadBlob(blob, 'alpaca-diary.csv');
        }
    } catch (e) {
        alert('Не удалось экспортировать');
    }
}

function escapeCsv(str) {
    if (!str) return '';
    return str.replace(/"/g, '""');
}

function downloadBlob(blob, filename) {
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
}

function showAlert(box, msg) {
    if (!box) return;
    box.style.display = 'block';
    box.textContent = msg;
}

function escapeHtml(text) {
    if (!text) return '';
    const map = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#039;'
    };
    return text.toString().replace(/[&<>"']/g, m => map[m]);
}
