const uri = 'api/Flats';
const port = 7045;
var server = `https://localhost:${port}/`;
var ruppinApi = 'https://proj.ruppin.ac.il/cgroup66/test2/tar1/' + uri;
const api = location.hostname === "localhost" || location.hostname === "127.0.0.1" ? server + uri : ruppinApi;
var isloggedIn = sessionStorage.getItem("loggedIn") === 'true' 

// AJAX GET api/Flats
function getFlats() {
    ajaxCall("GET", api, "", getSuccessCB, getErrCB);
}

function getSuccessCB(flats) {
    _renderFlats(flats);
}

function getErrCB(jqXHR, textStatus, errorThrown) {
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
}

// AJAX POST api/Flats
function submitFlat(event) {
    event.preventDefault(); 
    if (!isloggedIn)
        return;

    let Flat = {
        City: $("#city").val(),
        Address: $("#address").val(),
        NumberOfRooms: parseInt($("#rooms").val()),
        Price: parseFloat($("#price").val())
    };

    ajaxCall("POST", api, JSON.stringify(Flat), postSuccCB, postErrCB);
}

function postSuccCB(flat) {
    console.log(`newFlat = ` + JSON.stringify(flat));
    getFlats();
    Swal.fire({
        title: "Added Successfuly!",
        text: `A Flat with id of '${flat.Id}' is now available to order`,
        icon: "success"
    });
    clearInputValues();
}

function postErrCB(jqXHR, textStatus, errorThrown) {
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

function _renderFlats(flats) {
    $("#flats-list").empty();
    $("#flats-list").append(`<h2>Available Flats</h2>`);
    $("#flats-list").append(`<span id="flatInfoSpan"> Sign in to order vacations or to add a new flat</span>`);
    $("#flats-list").append('<div class="row">');
    flats.forEach(function (flat) {
        $("#flats-list .row").append(`
            <div class="col-md-3 mb-4">
                    <div class="flat card" id="${flat.Id}">
                        <div class="card-body">
                            <p class="card-text">Id: ${flat.Id}</p>
                            <p class="card-text">City: ${flat.City}</p>
                            <p class="card-text">Address: ${flat.Address}</p>
                            <p>Number of Rooms: ${flat.NumberOfRooms}</p>
                            <p>Price: ${flat.Price}</p>
                            <div class="card-footer orderBtn">
                                <button class="btn btn-primary" onclick="redirectToVacations(${flat.Id})">Order</button>
                            </div>
                        </div>
                    </div>
            </div>
            `);
    });
    if (isloggedIn) {
        $(".card-footer.orderBtn").show();
        $("#flatInfoSpan").hide();
    } else {
        $(".card-footer.orderBtn").hide();
        $("#flatInfoSpan").show();
    }
    $("#flats-list").append('</div>');
}

function clearInputValues() {
    $('#flatId, #city, #address, #rooms, #price').val('');
    $('#cities').find('option').prop('selected', false);
}

function redirectToVacations(flatId) {
    sessionStorage.setItem('selectedFlatId', flatId);
    window.location.href = "vacations.html";
}

function validateCityInput(input) {
    var datalist = document.getElementById('cities');
    var options = datalist.getElementsByTagName('option');
    var isValid = Array.from(options).some(function (option) {
        return option.value === input.value;
    });

    if (!isValid) {
        input.setCustomValidity('Please select a city from the list.');
        input.reportValidity();
    } else {
        input.setCustomValidity('');
    }
}