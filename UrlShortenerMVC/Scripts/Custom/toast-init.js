$(document).ready(function () {
    if ($("#NoError").val() == null) {
        $('.toast').toast(
            {
                autohide: false
            });
        $('.toast').toast('show');
        return;
    }
    $('.toast')[0].remove();
});