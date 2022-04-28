toastr.options = {
    closeButton: true,
    debug: false,
    newestOnTop: true,
    progressBar: true,
    rtl: false,
    positionClass: 'toast-top-right',
    onclick: null,
    timeOut: 15000
};

function successMsg(msg)
{
    toastr.success(msg, toastr.options);
}

function infoMsg(msg) {
    toastr.info(msg, toastr.options);
}

function warningMsg(msg) {
    toastr.warning(msg, toastr.options);
}

function errorMsg(msg) {
    toastr.error(msg, toastr.options);
}