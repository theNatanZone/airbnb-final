const users_uri = 'api/Users';
const localhost_port = 7045;
var local_server = `https://localhost:${localhost_port}/${users_uri}`;
var ruppin_server = `https://proj.ruppin.ac.il/cgroup66/test2/tar1/${users_uri}`;
const users_api = location.hostname === "localhost" || location.hostname === "127.0.0.1" ? local_server : ruppin_server;

function signinUser(event) {
    event.preventDefault(); 
    
    var userCredentials = {
        Email: $('#email').val(),
        Password: $('#pwd').val()
    };

    ajaxCall("POST", `${users_api}/login`, JSON.stringify(userCredentials), postLoginSuccCB, postLoginErrCB);
}

function postLoginSuccCB(response, status, xhr) {
    sessionStorage.setItem('loggedIn', true);
    isloggedIn = true;

    var passwordInput = $('#pwd').val();
    var userDetails = {
        Email: response.Email,
        FullName: response.FullName != undefined ? response.FullName : "",
        IsActive: response.IsActive ?? true,
        IsAdmin: response.IsAdmin ?? false,
        AuthToken: btoa(`${response.Email}:${passwordInput}`)
    }

    sessionStorage.setItem('userDetails', JSON.stringify(userDetails));
    updateUserMode();

    return Swal.fire({
        title: "Logged in Successfuly!",
        text: `You are now connected to '${response.Email}' account`,
        icon: "success"
    }).then(function () {
        if (userDetails.IsAdmin)
            window.location.href = 'admin.html';
        else window.location.href = 'flats.html';
    });
              
}

function postLoginErrCB(xhr, status, error) {
    sessionStorage.setItem('loggedIn', false);
    isloggedIn = false;
    updateUserMode();
    postUserErrCB(xhr, status, error);
}

function postRegisterSuccCB(user) {
    console.log(`new User:\n` + JSON.stringify(user));
    return Swal.fire({
        title: "Added Successfuly!",
        text: `User '${user.Email}' has created successfully!`,
        icon: "success",
        footer: `<p>To connect your account, <span id="footerSpan">please press on 'Sign In'</span> button.</p>`
    }).then(function () {
        window.location.href = 'flats.html';
    });
}

function updateUserSuccCB(user) {
    var passwordInput = $('#edit-pwd').val() ?? '';
    var authToken = getPropertyFromSessionStorage('AuthToken');
    if (passwordInput.length > 0) {
        authToken = btoa(`${user.Email}:${passwordInput}`);
    }

    var newUserDetails = {
        Email: user.Email,
        FullName: user.FullName ?? "",
        IsActive: user.IsActive,
        IsAdmin: user.IsAdmin,
        AuthToken: authToken
    }

    sessionStorage.setItem('userDetails', JSON.stringify(newUserDetails));

    return Swal.fire({
        title: "Updated Successfuly!",
        text: `User details for '${user.Email}' have updated successfully!`,
        icon: "success"
    }).then(function () {
        window.location.href = 'flats.html';
    });
}

function postUserErrCB(jqXHR, textStatus, errorThrown) {
    var msg = "";
    var statusCode = jqXHR.status;
    if (statusCode === 0) {
        msg = 'Not connected.\nVerify Network. ';
        console.error(msg);
    }
    else if (statusCode >= 400) {
        msg = jqXHR.responseText;
        console.error(`Server Error\nStatus: ${statusCode}.\nError Message: "${msg}"`);
    }
    else if (jqXHR.responseJSON && jqXHR.responseJSON.error) {
        console.error("Server Error:", jqXHR.responseJSON.error);
    }
    else {
        msg = 'Uncaught Error.\n' + jqXHR.responseText;
        console.error(msg);
    }

    Swal.fire({
        icon: "error",
        title: "Oops...",
        text: "Something went wrong!",
        footer: `<p id="swal-msg">${msg}</p>`
    });
} 

function signupUser(event) {
    event.preventDefault();

    let user = {
        FirstName: $('#reg-fname').val(),
        FamilyName: $('#reg-lname').val(), 
        Email: $('#reg-email').val(),
        Password: $('#reg-pwd').val(),
        IsActive: true,
        IsAdmin: false 
    };

    ajaxCall("POST", `${users_api}/register`, JSON.stringify(user), postRegisterSuccCB, postUserErrCB);
}

function updateUserDetails(event) {
    event.preventDefault();

    let isAdmin = getPropertyFromSessionStorage("IsAdmin");
    console.log(isAdmin);

    let newUser = {
        FirstName: $('#edit-fname').val(),
        FamilyName: $('#edit-lname').val(),
        Email: $('#edit-email').val(),
        Password: $('#edit-pwd').val() ?? '',
        IsActive: true,
        IsAdmin: isAdmin
    };

    ajaxCall("PUT", `${users_api}/update-details`, JSON.stringify(newUser), updateUserSuccCB, postUserErrCB);
}

function updateUserMode() {
    var loggedInMode = ( sessionStorage.getItem('loggedIn') === 'true' && sessionStorage.getItem('userDetails') != null );

    if (loggedInMode) {

        var currentUser = JSON.parse(sessionStorage.getItem('userDetails'));
        if (currentUser.IsAdmin == true) {
            $('#admin-page-btn').show();
        }

        $('#sign-in-btn').hide();
        $('#sign-up-btn').hide();
        $('#my-account-btn').show();
        $('#logout-btn').show();
        $("#newFlatForm").show();
        $("#add-flat-btn").show();

    } else {
        $('#sign-in-btn').show();
        $('#sign-up-btn').show();
        $('#my-account-btn').hide();
        $('#logout-btn').hide();
        $("div:has(#newFlatForm)").hide();
        $("#add-flat-btn").hide();

        $('#admin-page-btn').hide();
    }

}

function logout() {
    sessionStorage.setItem('loggedIn', false);
    sessionStorage.removeItem('userDetails');
    updateUserMode();
    window.location.href = 'flats.html'; // Redirect to flats page after logout
}

function isLoggedInAdmin() {
    var userDetailsString = sessionStorage.getItem('userDetails');
    var userDetails = JSON.parse(userDetailsString ?? "{}");
    var result = userDetails?.IsAdmin;
    console.log(result);
    return result == true;
}

function togglePasswordInput() {
    var passwordGroup = $('#password-group');
    var passwordInput = $('#edit-pwd');
    var changePasswordCheckbox = $('#change-password-checkbox');

    if (changePasswordCheckbox.prop('checked')) {
        passwordGroup.show();
        passwordInput.prop('required', true);
        passwordInput.attr('minlength', '5');
        passwordInput.attr('maxlength', '64');
    } else {
        passwordGroup.hide();
        passwordInput.prop('required', false);
        passwordInput.removeAttr('minlength');
        passwordInput.removeAttr('maxlength');
    }
}

