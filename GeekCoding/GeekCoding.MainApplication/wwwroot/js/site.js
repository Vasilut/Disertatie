// Write your JavaScript code.

$('#submissie').click(function () {
    ajaxFunction();
});

function ajaxFunction() {
    alert(confirmUrl);
    $.ajax({
        url: confirmUrl,
        contentType: 'application/html; charset=utf-8',
        data: { 'id': id,}
    })
        .success(function (result) {
            alert(result);
        })
        .error(function (xhr, status) {
            alert(status);
        })
}