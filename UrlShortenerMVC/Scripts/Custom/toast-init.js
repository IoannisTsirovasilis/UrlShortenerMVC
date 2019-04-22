$(document).ready(function () {
    if ($("#NoError").val() == null) {
        $('.toast').toast(
            {
                autohide: false
            });
        $('.toast').toast('show');
    }    
});