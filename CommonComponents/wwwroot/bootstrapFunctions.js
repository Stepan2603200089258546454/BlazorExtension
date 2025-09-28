export function openModal(id) {
    try {
        const modalElement = document.getElementById(id);
        const myModal = new bootstrap.Modal(modalElement);
        myModal.show();
        return true;
    }
    catch (err) {
        console.error(err);
        return false;
    }
}
export function closeModal(id) {
    try {
        const modalElement = document.getElementById(id);
        const myModal = new bootstrap.Modal(modalElement);
        myModal.hide();
        return true;
    }
    catch (err) {
        console.error(err);
        return false;
    }
}