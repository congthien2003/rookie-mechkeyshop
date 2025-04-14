// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const CART_KEY = "cartItems";

 function getCart() {
    const cart = localStorage.getItem(CART_KEY);
    return cart ? JSON.parse(cart) : [];
}

function saveCart(cart) {
    localStorage.setItem(CART_KEY, JSON.stringify(cart));
}

function addToCart(item) {
    let cart = getCart();
    const existing = cart.find(p => p.id === item.id);
    if (existing) {
        existing.quantity += item.quantity;
    } else {
        cart.push(item);
    }
    saveCart(cart);
    alert("Đã thêm vào giỏ hàng!");
}

function removeFromCart(productId) {
    let cart = getCart();
    cart = cart.filter(p => p.id !== productId);
    saveCart(cart);
}

function clearCart() {
    localStorage.removeItem(CART_KEY);
}

var btnAddToCart = document.querySelectorAll('.btn-add-to-cart');
btnAddToCart.forEach(function (btn) {
    btn.addEventListener('click', function (event) {
        event.preventDefault();
        const productId = this.getAttribute('data-product-id');
        const quantity = Number.parseInt(this.getAttribute('data-quantity'));
        const imageUrl = this.getAttribute('data-product-imageurl');
        const price = this.getAttribute('data-product-price');
        const productName = this.getAttribute('data-product-name');
        const data = { productId, productName, quantity, price, imageUrl };
        console.log(data);
        addToCart(data);
        console.log(getCart());
    });
});

function renderCart() {
    const cart = getCart();
    const container = document.getElementById("cartItemsContainer");
    if (cart.length === 0) {
        container.innerHTML = "<p>Giỏ hàng trống</p>";
        return;
    }

    let html = "<ul>";
    let total = 0;
    cart.forEach(item => {
        const subtotal = item.quantity * item.price;
        total += subtotal;
        html += `<li>${item.name} x ${item.quantity} = ${subtotal.toLocaleString()}₫</li>`;
    });
    html += "</ul>";
    html += `<p><strong>Tổng cộng:</strong> ${total.toLocaleString()}₫</p>`;
    container.innerHTML = html;
}