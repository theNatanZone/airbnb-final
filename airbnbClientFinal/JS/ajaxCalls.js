function ajaxCall(method, api, data, successCB, errorCB) {
    $.ajax({
        type: method,
        url: api,
        data: data,
        cache: false,
        contentType: "application/json",
        dataType: "json",
        success: successCB,
        error: errorCB
    });
}

function ajaxAuthCall(method, api, data, successCB, errorCB) {
    

    $.ajax({
        type: method,
        url: api,
        data: data,
        cache: false,
        contentType: "application/json",
        dataType: "json",
        beforeSend: function (xhr) {
            var token = getAuthToken();
            if (token) {
                xhr.setRequestHeader("Authorization", "Basic " + token);
            }
            else {
                errorCB(xhr, "Unauthorized", "AuthToken is missing");
            }
        },
        success: successCB,
        error: errorCB
    });
}

function getAuthToken() {
    var userDetailsString = sessionStorage.getItem('userDetails');
    var userDetails = JSON.parse(userDetailsString ?? "{}");
    var authToken = userDetails?.AuthToken;
    return authToken;
}
