function openModal(modalId) {
    document.getElementById(modalId).classList.remove('hidden');
}

function closeModal(modalId) {
    document.getElementById(modalId).classList.add('hidden');
    if (modalId.includes('delete')) {
        document.getElementById(`delete-${modalId.split('-')[1]}-id`).value = '';
    }
}

function openEditModal(type, id, name, description, price, stock, imagePath) {
    document.getElementById(`edit-${type}-id`).value = id;
    document.getElementById(`edit-${type}-name`).value = name;
    document.getElementById(`edit-${type}-description`).value = description;
    document.getElementById(`edit-${type}-price`).value = price.toString().replace('.', ',');
    document.getElementById(`edit-${type}-stock`).value = parseInt(stock, 10);
    document.getElementById(`edit-${type}-imagefile`).value = '';
    openModal(`edit-${type}-modal`);
}

function openDeleteModal(id, type) {
    if (!id || isNaN(id) || id <= 0) {
        console.error(`Invalid ID provided: ${id}`);
        alert('Ошибка: Неверный ID');
        return;
    }
    const input = document.getElementById(`delete-${type}-id`);
    if (!input) {
        console.error(`Input element delete-${type}-id not found`);
        alert('Ошибка: Не найден элемент формы');
        return;
    }
    input.value = id;
    openModal(`delete-${type}-modal`);
}

function handleCreateForm(type, gridId, endpoint) {
    document.getElementById(`create-${type}-form`).addEventListener('submit', async (e) => {
        e.preventDefault();
        const formData = new FormData(e.target);
        const price = formData.get('Price').replace(',', '.');
        formData.set('Price', price);
        formData.set('Stock', parseInt(formData.get('Stock'), 10));
        try {
            const response = await fetch(endpoint, {
                method: 'POST',
                body: formData
            });
            const result = await response.json();
            if (result.success) {
                const grid = document.getElementById(gridId);
                const card = document.createElement('div');
                card.className = 'bg-white rounded-xl shadow-lg p-4';
                card.dataset.id = result.data.id;
                card.innerHTML = `
                    <img src="${result.data.imagePath || '/Images/placeholder.png'}" alt="${result.data.name}" class="w-full h-48 object-cover rounded-lg mb-4" />
                    <h2 class="text-lg font-semibold text-gray-800">${result.data.name}</h2>
                    <p class="text-gray-600">₸${parseFloat(result.data.price).toFixed(2)}</p>
                    <p class="text-gray-600">На складе: ${result.data.stock}</p>
                    <div class="mt-4 flex space-x-2">
                        <button onclick="openEditModal('${type}', ${result.data.id}, '${result.data.name}', '${result.data.description}', ${parseFloat(result.data.price)}, ${result.data.stock}, '${result.data.imagePath}')" class="text-blue-500 hover:underline">Редактировать</button>
                        <button onclick="openDeleteModal(${result.data.id}, '${type}')" class="text-red-500 hover:underline">Удалить</button>
                    </div>
                `;
                grid.appendChild(card);
                closeModal(`create-${type}-modal`);
                alert(result.message);
            } else {
                alert('Ошибка: ' + (result.errors ? result.errors.join(', ') : result.message));
            }
        } catch (error) {
            alert(`Ошибка при создании ${type}: ` + error.message);
        }
    });
}

function handleEditForm(type, gridId, endpoint) {
    document.getElementById(`edit-${type}-form`).addEventListener('submit', async (e) => {
        e.preventDefault();
        const formData = new FormData(e.target);
        const price = formData.get('Price').replace(',', '.');
        formData.set('Price', price);
        formData.set('Stock', parseInt(formData.get('Stock'), 10));
        try {
            const response = await fetch(endpoint, {
                method: 'POST',
                body: formData
            });
            const result = await response.json();
            if (result.success) {
                const card = document.querySelector(`#${gridId} [data-id="${result.data.id}"]`);
                if (!card) {
                    console.error(`Card with data-id="${result.data.id}" not found in #${gridId}`);
                    alert('Ошибка: Карточка не найдена на странице');
                    return;
                }
                card.innerHTML = `
                    <img src="${result.data.imagePath || '/Images/placeholder.png'}" alt="${result.data.name}" class="w-full h-48 object-cover rounded-lg mb-4" />
                    <h2 class="text-lg font-semibold text-gray-800">${result.data.name}</h2>
                    <p class="text-gray-600">₸${parseFloat(result.data.price).toFixed(2)}</p>
                    <p class="text-gray-600">На складе: ${result.data.stock}</p>
                    <div class="mt-4 flex space-x-2">
                        <button onclick="openEditModal('${type}', ${result.data.id}, '${result.data.name}', '${result.data.description}', ${parseFloat(result.data.price)}, ${result.data.stock}, '${result.data.imagePath}')" class="text-blue-500 hover:underline">Редактировать</button>
                        <button onclick="openDeleteModal(${result.data.id}, '${type}')" class="text-red-500 hover:underline">Удалить</button>
                    </div>
                `;
                closeModal(`edit-${type}-modal`);
                alert(result.message);
            } else {
                alert('Ошибка: ' + (result.errors ? result.errors.join(', ') : result.message));
            }
        } catch (error) {
            alert(`Ошибка при обновлении ${type}: ` + error.message);
        }
    });
}

function handleDeleteForm(type, gridId, endpoint) {
    document.getElementById(`delete-${type}-form`).addEventListener('submit', async (e) => {
        e.preventDefault();
        const submitButton = document.getElementById(`delete-${type}-submit`);
        if (submitButton.disabled) return;
        submitButton.disabled = true;
        submitButton.textContent = 'Удаление...';
        const id = parseInt(document.getElementById(`delete-${type}-id`).value, 10);
        if (isNaN(id) || id <= 0) {
            alert(`Ошибка: Неверный ID ${type}`);
            submitButton.disabled = false;
            submitButton.textContent = 'Удалить';
            return;
        }
        try {
            const response = await fetch(endpoint, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ id })
            });
            if (!response.ok) {
                const text = await response.text();
                throw new Error(`HTTP ${response.status}: ${text}`);
            }
            const result = await response.json();
            if (result.success) {
                const card = document.querySelector(`#${gridId} [data-id="${id}"]`);
                if (card) card.remove();
                closeModal(`delete-${type}-modal`);
                alert(result.message);
            } else {
                alert('Ошибка: ' + result.message);
            }
        } catch (error) {
            alert(`Ошибка при удалении ${type}: ` + error.message);
        } finally {
            submitButton.disabled = false;
            submitButton.textContent = 'Удалить';
        }
    });
}

function loadStatistics() {
    fetch('/Admin/Statistics')
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // График заказов за неделю
                const ordersCtx = document.getElementById('ordersChart').getContext('2d');
                new Chart(ordersCtx, {
                    type: 'line',
                    data: {
                        labels: data.ordersByDay.labels,
                        datasets: [{
                            label: 'Количество заказов',
                            data: data.ordersByDay.values,
                            borderColor: '#16a34a',
                            backgroundColor: 'rgba(22, 163, 74, 0.2)',
                            fill: true,
                            tension: 0.4
                        }]
                    },
                    options: {
                        responsive: true,
                        scales: {
                            y: { beginAtZero: true, precision: 0 }
                        }
                    }
                });

                // График топ-5 товаров
                const topItemsCtx = document.getElementById('topItemsChart').getContext('2d');
                new Chart(topItemsCtx, {
                    type: 'doughnut',
                    data: {
                        labels: data.topItems.labels,
                        datasets: [{
                            data: data.topItems.values,
                            backgroundColor: ['#16a34a', '#3b82f6', '#ef4444', '#f59e0b', '#8b5cf6']
                        }]
                    },
                    options: {
                        responsive: true,
                        plugins: {
                            legend: { position: 'right' }
                        }
                    }
                });
            } else {
                console.error('Ошибка загрузки статистики:', data.message);
                alert('Ошибка загрузки статистики: ' + data.message);
            }
        })
        .catch(error => {
            console.error('Ошибка:', error);
            alert('Ошибка при загрузке статистики: ' + error.message);
        });
}