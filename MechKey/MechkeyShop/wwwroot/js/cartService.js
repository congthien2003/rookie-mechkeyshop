const CART_KEY = "cartItems";

const CountCartItems = document.getElementById("counting-cart");
const updateCartCount = function () {
    const cart = getCart();
    const count = cart.reduce((total, item) => total + item.quantity, 0);
    CountCartItems.innerText = count;
}

const updateTotalPrice = function (cart) {
    const totalPrice = document.getElementById('total-price');
    const total = cart.reduce((total, item) => total + item.quantity * item.price, 0);
    totalPrice.innerText = total;
}

const getCart = function () {
    const cart = localStorage.getItem(CART_KEY);
    return cart ? JSON.parse(cart) : [];
}

updateCartCount();


const saveCart = function (cart) {
    localStorage.setItem(CART_KEY, JSON.stringify(cart));
    updateCartCount();
}

const addToCart = function (item) {
    let cart = getCart();
    console.log(item);

    let existing;
    if (item.option) {
        existing = cart.find(p => p.productId === item.productId && p.option.id === item.option.id && p.option.value === item.option.value);
    }
    else {
        existing = cart.find(p => p.productId === item.productId);
    }

    if (existing) {
        existing.quantity += item.quantity;
    } else {
        cart.push(item);
    }
    showNoti("success", '<i class="fa-solid fa-check me-2"></i> Added to cart', `Item has been added to your cart`);
    saveCart(cart);
}

const removeFromCart = function (productId) {
    let cart = getCart();
    cart = cart.filter(p => p.productId !== productId);
    saveCart(cart);
    renderCart();

}

const clearCart = function () {
    updateCartCount();
    localStorage.removeItem(CART_KEY);
}

const updateQuantity = function (event, productId) {
    let cart = getCart();
    console.log(cart);
    const existing = cart.find(p => p.productId === productId);
    existing.quantity = Number.parseInt(event.target.value);
    saveCart(cart);
    renderCart();
}

var renderCart = function () {
    const cart = getCart();
    const container = document.getElementById("cart-items");
    if (cart.length === 0) {
        container.innerHTML = `<p class="text-center fw-bold text-danger">Cart is empty. </br> Go to shop now !</p>`;
        return;
    }

    let html = "<ul>";
    let total = 0;
    cart.forEach(item => {
        const subtotal = item.quantity * item.price;
        total += subtotal;
        html += `<li class="item">
                            <div class="item--img">
                                <img src="${item.imageUrl}" />
                            </div>
                            <div class="item--info">
                                <p class="info-name">${item.productName}</p>
                                <p class="info-category">${item.option ? `${item.option.id} : ${item.option.value}` : "None"} </p>
                            </div>
                            <div class="item--price">
                               $ ${item.price}
                            </div>
                            <span>X</span>
                            <div class="item--quantity">
                                <input class="form-control" type="number" value=${item.quantity} min="1" onchange="updateQuantity(event, '${item.productId}')" />
                                </div>
                            <div class="item--total">
                                $ ${(item.price * item.quantity).toLocaleString("vi-VN")}
                            </div>
                            <button class="btn btn-sm" onclick="removeFromCart('${item.productId}')">
                                <i class="fa-solid fa-trash"></i>
                            </button>
                        </li>`;
    });
    html += "</ul>";
    container.innerHTML = html;
    updateTotalPrice(cart);
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
        addToCart(data);
    });
});
