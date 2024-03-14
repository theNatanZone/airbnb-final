const uri = 'api/Vacations';
const port = 7045;
var server = `https://localhost:${port}/`;
var ruppinApi = 'https://proj.ruppin.ac.il/cgroup66/test2/tar1/' + uri;
const api = location.hostname === "localhost" || location.hostname === "127.0.0.1" ? server + uri : ruppinApi;
const isloggedIn = sessionStorage.getItem("loggedIn") === 'true' ? true : false;

// AJAX GET api/Vacations
function getVacationsByUser() {
    if (!isloggedIn)
        return;
    let userId = $("#userId").val();    
    let route = `${api}/user-vacations?userId=${userId}`;
    ajaxCall("GET", route, "", getSuccessCB, getErrCB);
}

function getSuccessCB(vacations) {
    const userId = $("#userId").val();
    var filteredVacations = vacations;
    console.log('Filtered Vacations: \n' + JSON.stringify(filteredVacations));

    if (filteredVacations.length == 0) {
        return Swal.fire({
            icon: "error",
            title: "Sorry...",
            text: `No vacations have been found for user "${userId}"`,
            footer: `<p id="swal-msg" class="vacation-not-found">Add a new vacation and try again.</p>`
        }).then(function () {
            $("#vacations-list").empty();
            $("#vacations-list").append(`<h2>Existing Vacations-</h2><p class="noRes">No vacations have been found for user "${userId}".</p>`);
            $("#vacations-list").append(`<p>* Make sure the User Id is correct and that you have any existing vacations.</p>`);
        });
    }
    _renderVacations(filteredVacations, userId);
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
        if (statusCode == 404) {
            getSuccessCB(new Array());
        }
    }
    else if (jqXHR.responseJSON && jqXHR.responseJSON.error) {
        console.error("Server Error:", jqXHR.responseJSON.error);
    }
    else {
        msg = 'Uncaught Error.\n' + jqXHR.responseText;
        console.error(msg);
    }
}


// AJAX POST api/Vacations
function submitVacation(event) {
    event.preventDefault();

    let Vacation = {
        Id: parseInt($("#vacationId").val()),
        UserId: $("#userId").val(),
        FlatId: parseInt($("#flatId").val()),
        StartDate: $("#startDate").val(),
        EndDate: $("#endDate").val()
    };

    ajaxCall("POST", api, JSON.stringify(Vacation), postVacationSuccCB, postVacationErrCB);
}

function postVacationSuccCB(vacation) {
    console.log(`new Vacation:\n` + JSON.stringify(vacation));
    return Swal.fire({
        title: "Added Successfuly!",
        text: `A Vacation with Id of '${vacation.Id}' and flat Id of '${vacation.FlatId}' has been added`,
        icon: "success",
        footer: `<p>To view the new order, <span id="footerSpan">please press on 'Get All My Vacations'</span> button.</p>`
    });
}

function postVacationErrCB(jqXHR, textStatus, errorThrown) {
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


function displayVacationForm(selectedFlatId, userId) {
    $("#flatId").val(selectedFlatId);
    $("#userId").val(userId);
    $("#newVacationForm").show();
    $("#vacations-list").show();

    $(':input[type="button"]').prop('disabled', true);
    if ($("#userId").val() != "") {
        $(':input[type="button"]').prop('disabled', false);
    }
    $('#userId').on('input', function () {
        $(':input[type="button"]').prop('disabled', true);
    });

    $('#userId').on('change', function () {
        if ($(this).val() != '' && $(this)[0].validity.valid === true) {
            $(':input[type="button"]').prop('disabled', false);
        }
        else {
            $(':input[type="button"]').prop('disabled', true);
        }
    });

    $("#newVacationForm").submit(submitVacation);
    $('#getVacationsBtn').on('click', getVacationsByUser); 
}

function initializeDatePickers() {
    var from = $("#startDate")
            .datepicker({
                defaultDate: "+1w",
                changeMonth: true,
                numberOfMonths: 2
            })
            .on("change", function () {
                to.datepicker("option", "minDate", getDate(this));
                checkDatesValidity();

                if ($(this).val() == to.val()) {
                    $(this).datepicker("option", "maxDate", getDate(this));
                }
                // report format validity when the value is null
                if (!$(this).val()) { 
                    this.reportValidity();
                }
            }),
        to = $("#endDate").datepicker({
            defaultDate: "+1w",
            changeMonth: true,
            numberOfMonths: 2
        })
            .on("change", function () {
                from.datepicker("option", "maxDate", getDate(this));
                checkDatesValidity();

                if ($(this).val() == from.val()) {
                    $(this).datepicker("option", "minDate", getDate(this));
                }
                
                // report format validity when the value is null
                if (!$(this).val()) {
                    this.reportValidity();
                }
            });

    $(".vacation-dates").datepicker("option", "dateFormat", "yy-mm-dd");
    $("#endDate").on("input", function () {
        this.setCustomValidity('');
    });

    $("#startDate").on("input", function () {
        this.setCustomValidity('');
    });
}

function getDate(element) {
    var date;
    try {
        // try parse with datepicker "yy-mm-dd" date format
        date = $.datepicker.parseDate("yy-mm-dd", element.value);
    } catch (error) {
        date = null;
        element.value = null;
    }

    return date;
}

function checkDatesValidity() {
    const startDate = $('#startDate').datepicker('getDate');
    const endDate = $('#endDate').datepicker('getDate');
    const startDateInput = $('#startDate')[0];
    const endDateInput = $('#endDate')[0];

    if (!startDate || !endDate) {
        if (!startDate) {
            startDateInput.setCustomValidity('Please fill out this field with the format: "yyyy-mm-dd"');

        } else {
            startDateInput.setCustomValidity('');
        }

        if (!endDate) {
            endDateInput.setCustomValidity('Please fill out this field with the format: "yyyy-mm-dd"');
        } else {
            endDateInput.setCustomValidity('');
        }

        return;
    }

    let maxDate = new Date(startDate);
    maxDate.setDate(maxDate.getDate() + 20);

    if (endDate > maxDate) {
        endDateInput.setCustomValidity('End-Date is not valid! The vacation cannot exceed 20 days');
    } else if (endDate <= startDate) {
        endDateInput.setCustomValidity('End date is not valid! End-Date should be greater than Start-Date.');
    } else {
        endDateInput.setCustomValidity('');
    }

    startDateInput.setCustomValidity('');

    endDateInput.reportValidity();
    startDateInput.reportValidity();
}

function _renderVacations(vacations, userId) {
    $("#vacations-list").empty();
    $("#vacations-list").append(`<h2>Existing Vacations-</h2><h5>User Id: ${userId}</h5>`);
    $("#vacations-list").append('<div class="row">');
    vacations.forEach(function (vacation) {
        $("#vacations-list .row").append(`
            <div class="col-md-3 mb-4">
                    <div class="vacation card" id="${vacation.Id}">
                        <div class="card-body">
                            <p class="card-text">Id: ${vacation.Id}</p>
                            <p class="card-text">User Id: ${vacation.UserId}</p>
                            <p class="card-text">Flat Id: ${vacation.FlatId}</p>
                            <p>Start Date: ${vacation.StartDate}</p>
                            <p>End Date: ${vacation.EndDate}</p>
                        </div>
                    </div>
            </div>
            `);
    });
    $("#vacations-list").append('</div>');
    $("#vacations-list").show();
}

function resetVacationsList() {
    $("#vacations-list").empty();
    $("#vacations-list").append(`<h2>Existing Vacations-</h2><p>* Please fill in the "User Id" field and press on "Get All My Vacations".</p>`);
}