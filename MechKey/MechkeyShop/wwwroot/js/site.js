
// Submit Review Handler
function getCookie(name) {
    const match = document.cookie.match(new RegExp('(^| )' + name + '=([^;]+)'));
    if (match) return match[2];
    return null;
}

function showLoginModal() {
    const loginModal = new bootstrap.Modal(document.getElementById('loginRequiredModal'));
    loginModal.show();
}

function showNoti(type, header, message) {
    const liveToast = document.getElementById('liveToast');

    const toast = new bootstrap.Toast(liveToast, {
        autohide: true,
        delay: 3000
    });
    toast.show();
    const toastHeader = document.querySelector('.toast-header');
    const toastBody = document.querySelector('.toast-body');
    toastHeader.innerHTML = header;
    toastBody.innerHTML = message;
    toastHeader.classList.remove('success', 'error', 'info');

    switch (type) {
        case 'success':
            toastHeader.classList.add('success');
            break;
        case 'error':
            toastHeader.classList.add('error');
            break;
        default:
            toastHeader.classList.add('info');
            break;
    }
}