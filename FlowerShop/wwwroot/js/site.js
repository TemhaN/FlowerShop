document.addEventListener('DOMContentLoaded', () => {
    const cartToggle = document.getElementById('cart-toggle');
    const cartToggleMobile = document.getElementById('cart-toggle-mobile');
    const cartSidebar = document.getElementById('cart-sidebar');
    const cartClose = document.getElementById('cart-close');
    const mainContainer = document.getElementById('main-container');
    const header = document.querySelector('header');
    const logoutButton = document.getElementById('logout-button');
    const searchForm = document.getElementById('search-form');
    const resetFilters = document.getElementById('reset-filters');
    const productsContainer = document.getElementById('products-container');
    const loadingSpinner = document.getElementById('loading-spinner');
    const dropdowns = document.querySelectorAll('.dropdown');

    let isLoading = false;

    // Dropdown обработка
    dropdowns.forEach(dropdown => {
        const toggle = dropdown.querySelector('.dropdown-toggle');
        const menu = dropdown.querySelector('.dropdown-menu');
        const chevron = dropdown.querySelector('.fa-chevron-down');
        let hideTimeout;

        if (toggle && menu) {
            dropdown.addEventListener('mouseenter', () => {
                console.log('Dropdown mouseenter:', toggle.textContent);
                clearTimeout(hideTimeout);
                dropdowns.forEach(d => {
                    const m = d.querySelector('.dropdown-menu');
                    const c = d.querySelector('.fa-chevron-down');
                    if (m && m !== menu) m.classList.remove('show');
                    if (c && c !== chevron) c.classList.remove('rotate');
                });
                menu.classList.add('show');
                if (chevron) chevron.classList.add('rotate');
            });

            dropdown.addEventListener('mouseleave', () => {
                hideTimeout = setTimeout(() => {
                    console.log('Dropdown mouseleave:', toggle.textContent);
                    menu.classList.remove('show');
                    if (chevron) chevron.classList.remove('rotate');
                }, 200);
            });

            menu.addEventListener('mouseenter', () => {
                clearTimeout(hideTimeout);
            });

            menu.addEventListener('mouseleave', () => {
                hideTimeout = setTimeout(() => {
                    menu.classList.remove('show');
                    if (chevron) chevron.classList.remove('rotate');
                }, 200);
            });
        }
    });

    // Открыть корзину
    function openCart() {
        console.log('Opening cart');
        cartSidebar.classList.add('open');
        cartSidebar.classList.remove('closed');
        document.getElementById('main-container').classList.add('shifted');
    }

    // Закрыть корзину
    function closeCart() {
        console.log('Closing cart');
        cartSidebar.classList.remove('open');
        cartSidebar.classList.add('closed');
        document.getElementById('main-container').classList.remove('shifted');
    }

    // Закрытие корзины при клике вне её на мобильных
    document.addEventListener('click', (e) => {
        if (window.innerWidth <= 900 && cartSidebar.classList.contains('open')) {
            const isClickInsideCart = cartSidebar.contains(e.target);
            const isClickOnToggle = cartToggleMobile?.contains(e.target) || cartToggle?.contains(e.target);
            if (!isClickInsideCart && !isClickOnToggle) {
                closeCart();
            }
        }
    });

    // Показ уведомления
    function showNotification(message, isError = false) {
        console.log('Showing notification:', message, 'isError:', isError);
        const div = document.createElement('div');
        div.className = `notification ${isError ? 'error' : 'success'}`;
        div.textContent = message;
        const notificationContainer = document.getElementById('notification-container');
        if (!notificationContainer) {
            console.error('Notification container not found!');
            return;
        }

        // Устанавливаем начальный индекс и позицию
        const notifications = notificationContainer.querySelectorAll('.notification');
        const index = notifications.length; // Индекс нового уведомления
        div.style.setProperty('--index', index);
        div.style.bottom = '-50px'; // Начальная позиция ниже видимой области
        div.style.opacity = '1'; // Новое уведомление полностью непрозрачное
        div.style.transform = 'translateX(-50%) scale(1)'; // Начальный размер

        notificationContainer.appendChild(div);
        console.log('Notification div added:', div.outerHTML);

        // Удаляем самое старое уведомление, если их больше 5
        if (notifications.length >= 5) {
            notifications[0].remove();
            console.log('Removed oldest notification due to limit of 5');
        }

        // Обновляем позиции, прозрачность и размер всех уведомлений
        const updatePositions = () => {
            const allNotifications = notificationContainer.querySelectorAll('.notification');
            allNotifications.forEach((notif, i) => {
                const offset = (allNotifications.length - 1 - i) * 10; // 25px — высота уведомления + отступ
                notif.style.bottom = `${offset + 10}px`; // Целевая позиция
                notif.style.zIndex = 5000 + (allNotifications.length - 1 + i); // Твой zIndex, новые сверху
                // Устанавливаем прозрачность: новое — 1, старые уменьшаются
                const opacity = 1 - (allNotifications.length - 1 - i) * 0.2; // Уменьшаем на 0.2
                notif.style.opacity = Math.max(opacity, 0.4); // Минимум 0.4
                // Устанавливаем масштаб: новое — 1, старые уменьшаются
                const scale = 1 - (allNotifications.length - 1 - i) * 0.1; // Уменьшаем на 0.1
                notif.style.transform = `translateX(-50%) scale(${Math.max(scale, 0.2)})`; // Минимум 0.7
            });
        };

        // Показываем уведомление
        setTimeout(() => {
            div.classList.add('show');
            console.log('Added show class:', div.className);
            updatePositions(); // Обновляем после добавления show для анимации
        }, 10); // Небольшая задержка для корректной анимации

        // Удаляем уведомление с анимацией
        setTimeout(() => {
            div.classList.remove('show');
            console.log('Removed show class:', div.className);
            setTimeout(() => {
                div.remove();
                console.log('Notification removed:', message);
                updatePositions(); // Обновляем позиции, прозрачность и размер после удаления
            }, 300); // Время анимации исчезновения
        }, 3000); // Время отображения
    }

    // Обновить итоговую сумму
    function updateCartTotal() {
        let total = 0;
        document.querySelectorAll('li[data-item-id]').forEach(li => {
            const quantity = parseInt(li.querySelector('.quantity').value) || 1;
            const price = parseFloat(li.dataset.price) || 0;
            const itemTotal = quantity * price;
            li.querySelector('.item-total').textContent = `₸${itemTotal.toFixed(2)}`;
            total += itemTotal;
        });
        const cartTotal = document.querySelector('.cart-total');
        if (cartTotal) cartTotal.textContent = `Итого: ₸${total.toFixed(2)}`;
    }

    // Обновить корзину
    async function refreshCart(retry = true) {
        console.log('Refreshing cart...');
        try {
            const response = await fetch('/Cart/GetCartSummary', {
                headers: { 'Cache-Control': 'no-cache' }
            });
            const cartHtml = await response.text();
            console.log('Cart summary received:', cartHtml);
            document.querySelector('.cart-summary').innerHTML = cartHtml;
            updateCartTotal();
            if (retry && !cartHtml.includes('data-item-id')) {
                console.log('Cart is empty, retrying in 1s...');
                await new Promise(resolve => setTimeout(resolve, 1000));
                await refreshCart(false);
            }
        } catch (error) {
            console.error('Error refreshing cart:', error.message);
            showNotification('Ошибка обновления корзины: ' + error.message, true);
        }
    }

    // Загрузка товаров
    async function loadProducts(page, append = false) {
        if (isLoading) return;
        isLoading = true;
        loadingSpinner?.classList.remove('hidden');

        const formData = {};
        for (const element of searchForm?.elements || []) {
            if (!element.name || element.disabled) continue;
            if (element.type === 'checkbox') {
                formData[element.name] = element.checked;
            } else if (element.type === 'radio' && !element.checked) {
                continue;
            } else {
                formData[element.name] = element.value;
            }
        }
        formData.Page = page;
        formData.Size = 12;

        console.log('Отправляемые данные:', JSON.stringify(formData, null, 2));

        try {
            const response = await fetch('/Home/Index', {
                method: 'POST',
                body: JSON.stringify(formData),
                headers: {
                    'Content-Type': 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            console.log('Статус ответа:', response.status, response.statusText);

            const html = await response.text();
            console.log('Полученный HTML:', html);

            const tempDiv = document.createElement('div');
            tempDiv.innerHTML = html;
            const newProducts = tempDiv.querySelectorAll('#products-container > div');
            const newPage = tempDiv.querySelector('#products-container')?.dataset.page;
            const newHasMore = tempDiv.querySelector('#products-container')?.dataset.hasMore;

            console.log('Количество новых продуктов:', newProducts.length);
            console.log('Новая страница:', newPage);
            console.log('Есть ещё продукты:', newHasMore);

            if (append) {
                newProducts.forEach(product => productsContainer.appendChild(product));
            } else {
                productsContainer.innerHTML = '';
                newProducts.forEach(product => productsContainer.appendChild(product));
            }

            productsContainer.dataset.page = newPage || page;
            productsContainer.dataset.hasMore = newHasMore || 'false';
            attachFormHandlers();
        } catch (error) {
            console.error('Ошибка в loadProducts:', error);
            showNotification('Ошибка загрузки товаров: ' + error.message, true);
        } finally {
            isLoading = false;
            loadingSpinner?.classList.add('hidden');
        }
    }

    // Обработка скролла
    function handleScroll() {
        if (isLoading || productsContainer.dataset.hasMore !== 'true') return;
        const rect = productsContainer.getBoundingClientRect();
        const isAtBottom = rect.bottom <= window.innerHeight + 100;
        if (isAtBottom) {
            const nextPage = parseInt(productsContainer.dataset.page) + 1;
            loadProducts(nextPage, true);
        }
    }

    let submitHandler = null;

    function attachFormHandlers() {
        console.log('Attaching form handlers...');
        if (submitHandler) {
            document.removeEventListener('submit', submitHandler);
            console.log('Removed previous submit handler');
        }

        submitHandler = (e) => {
            if (e.target.classList.contains('add-to-cart-form')) {
                console.log('Form with add-to-cart-form class submitted');
                handleCartForm(e);
            } else if (e.target.classList.contains('add-to-favorite-form')) {
                console.log('Form with add-to-favorite-form class submitted');
                handleFavoriteFormSubmit(e);
            }
        };

        document.addEventListener('submit', submitHandler);
    }

    // Обработчик формы корзины
    async function handleCartForm(e) {
        console.log('handleCartForm called');
        e.preventDefault();
        const form = e.target;
        const formData = new FormData(form);
        const id = formData.get('id');
        console.log(`Submitting to ${form.action} with id=${id}`);
        try {
            const response = await fetch(form.action, {
                method: 'POST',
                body: formData,
                headers: { 'Accept': 'application/json' }
            });
            const result = await response.json();
            console.log(`Response from ${form.action}:`, result);
            console.log(`Result.success: ${result.success}, Result.message: "${result.message}"`);
            showNotification(result.message, !result.success);
            if (result.success) {
                console.log('Processing successful cart addition');
                // Открываем корзину только на десктопе
                if (window.innerWidth > 900) {
                    console.log('Calling openCart');
                    openCart();
                }
                await refreshCart();
            }
        } catch (error) {
            console.log(`Error in ${form.action}: ${error.message}`);
            showNotification('Ошибка: ' + error.message, true);
        }
    }

    let isFavoriteSubmitting = false;

    async function handleFavoriteFormSubmit(e) {
        if (isFavoriteSubmitting) {
            console.log('Favorite form submission blocked: already in progress');
            return;
        }

        isFavoriteSubmitting = true;
        e.preventDefault();
        const form = e.target;
        const formData = new FormData(form);
        const button = form.querySelector('.favorite-btn');
        const svg = button.querySelector('svg');
        const isFavorite = form.dataset.isFavorite === 'true';
        console.log(`Submitting favorite form to ${form.action}, triggered by:`, e.isTrusted ? 'user' : 'script');

        try {
            const response = await fetch(form.action, {
                method: 'POST',
                body: formData,
                headers: { 'Accept': 'application/json' }
            });
            const result = await response.json();
            console.log(`Response from ${form.action}:`, result);
            showNotification(result.message, !result.success);

            if (result.success) {
                const id = formData.get('id');
                const controller = form.action.includes('Toys') ? 'Toys' : form.action.includes('Flowers') ? 'Flowers' : 'Bouquets';
                const statusResponse = await fetch(`/api/${controller}/IsFavorite?id=${id}`, {
                    headers: { 'Accept': 'application/json' }
                });
                const statusResult = await statusResponse.json();
                const newIsFavorite = statusResult.isFavorite;

                form.dataset.isFavorite = newIsFavorite.toString();
                svg.classList.toggle('text-red-600', newIsFavorite);
                svg.classList.toggle('text-gray-400', !newIsFavorite);
                svg.setAttribute('fill', newIsFavorite ? 'currentColor' : 'none');
                svg.setAttribute('stroke', newIsFavorite ? 'none' : 'currentColor');
            }
        } catch (error) {
            console.log(`Error in ${form.action}: ${error.message}`);
            showNotification('Ошибка: ' + error.message, true);
        } finally {
            isFavoriteSubmitting = false;
        }
    }

    // Обработка событий корзины
    document.addEventListener('click', async (e) => {
        const target = e.target;
        const itemId = target.dataset.itemId;

        if (target.classList.contains('increment')) {
            const input = target.parentElement.querySelector('.quantity');
            const newQuantity = parseInt(input.value) + 1;
            input.value = newQuantity;
            await updateCartItem(itemId, newQuantity);
        }

        if (target.classList.contains('decrement')) {
            const input = target.parentElement.querySelector('.quantity');
            const newQuantity = parseInt(input.value) - 1;
            if (newQuantity >= 1) {
                input.value = newQuantity;
                await updateCartItem(itemId, newQuantity);
            } else {
                showNotification('Количество не может быть меньше 1', true);
            }
        }

        if (target.classList.contains('remove')) {
            try {
                const response = await fetch('/api/Cart/RemoveCartItem', {
                    method: 'POST',
                    body: new URLSearchParams({ cartItemId: itemId }),
                    headers: { 'Accept': 'application/json' }
                });
                const result = await response.json();
                showNotification(result.message, !result.success);
                if (result.success) {
                    document.querySelector(`li[data-item-id="${itemId}"]`)?.remove();
                    await refreshCart();
                }
            } catch (error) {
                showNotification('Ошибка: ' + error.message, true);
            }
        }
    });

    // Изменение количества вручную
    document.addEventListener('change', async (e) => {
        if (e.target.classList.contains('quantity')) {
            const itemId = e.target.dataset.itemId;
            const quantity = parseInt(e.target.value);
            if (quantity >= 1) {
                await updateCartItem(itemId, quantity);
            } else {
                e.target.value = 1;
                showNotification('Количество не может быть меньше 1', true);
            }
        }
    });

    // Обновление количества
    async function updateCartItem(itemId, quantity) {
        try {
            const response = await fetch('/api/Cart/UpdateCartItem', {
                method: 'POST',
                body: new URLSearchParams({ cartItemId: itemId, quantity }),
                headers: { 'Accept': 'application/json' }
            });
            const result = await response.json();
            showNotification(result.message, !result.success);
            if (result.success) {
                await refreshCart();
            }
        } catch (error) {
            showNotification('Ошибка: ' + error.message, true);
        }
    }

    // Обработка выхода
    logoutButton?.addEventListener('click', async (e) => {
        e.preventDefault();
        try {
            const response = await fetch('/Auth/Logout', {
                method: 'POST',
                headers: { 'Accept': 'text/plain; charset=utf-8' }
            });
            const text = await response.text();
            showNotification(text, !response.ok);
            if (response.ok) {
                setTimeout(() => window.location.href = '/', 1000);
            }
        } catch (error) {
            showNotification('Ошибка: ' + error.message, true);
        }
    });

    // Обработка формы поиска
    searchForm?.addEventListener('change', () => loadProducts(1));
    resetFilters?.addEventListener('click', () => {
        searchForm.reset();
        loadProducts(1);
    });

    // Обработка скролла
    window.addEventListener('scroll', handleScroll);

    // Обработка событий корзины
    [cartToggle, cartToggleMobile].forEach(toggle => {
        if (toggle) {
            toggle.addEventListener('click', () => {
                if (cartSidebar.classList.contains('open')) {
                    closeCart();
                } else {
                    openCart();
                }
            });
        }
    });

    if (cartClose) {
        cartClose.addEventListener('click', closeCart);
    }

    // Инициализация обработчиков форм
    attachFormHandlers();

    // Инициализация начального состояния корзины
    cartSidebar.classList.add('closed');
});