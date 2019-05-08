$(document).ready(function () {
    toggleExpiresAtField();
})

function toggleExpiresAtField() {
    if ($("#Expires") != null) {
        if ($("#Expires").is(":checked")) {
            $("#ExpiresAtString").prop("disabled", false);
        }
        else {
            $("#ExpiresAtString").prop("disabled", true);
        }        
    }
}