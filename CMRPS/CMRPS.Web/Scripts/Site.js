// =====================================================
// Details Modal
// =====================================================
function showDetailsModal(id, action) {
    $("#modal-body").html("Please wait! Loading...");

    $.ajax({
        type: 'GET',
        url: action,
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        data: { Id: id },
        success: function (result) {
            $("#modal-body").html(result);
        },
        error: function () {
            $("#modal-body").html("Oops.. Something went wrong! :-(");
        }
    });
}

// =====================================================
// Delete Modal
// =====================================================
function showDeleteModal(id, action) {
    $("#modal-body-delete").html("Please wait! Loading...");
    $("#delete-item-id").html(id);

    $.ajax({
        type: 'GET',
        url: action,
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        data: { Id: id },
        success: function (result) {
            $("#modal-body-delete").html(result);
        },
        error: function () {
            $("#modal-body-delete").html("Oops.. Something went wrong! :-(");
        }
    });
}

function deleteItem(action) {
    var id = $("#delete-item-id").html();

    $.ajax({
        type: 'POST',
        url: action,
        data: { Id: id },
        success: function (result) {
            location.reload();
        },
        error: function () {
            alert("Oops.. Something went wrong! :-(");
        }
    });
}

// =====================================================
// Delete USER Modal
// =====================================================
function showDeleteUserModal(id, action) {
    $("#modal-body-delete").html("Please wait! Loading...");
    $("#delete-item-id").html(id);

    $.ajax({
        type: 'GET',
        url: action,
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        data: { Id: id },
        success: function (result) {
            $("#modal-body-delete").html(result);
        },
        error: function () {
            $("#modal-body-delete").html("Oops.. Something went wrong! :-(");
        }
    });
}

function deleteUser(action) {
    var id = $("#delete-item-id").html();
    var logins = $("#delete-item-logins").is(':checked');
    var events = $("#delete-item-events").is(':checked');

    console.log('ID: ' + id);
    console.log('Logins: ' + logins);
    console.log('Events: ' + events);

    $.ajax({
        type: 'POST',
        url: action,
        data: { Id: id, removeLogins: logins, removeEvents: events },
        success: function (result) {
            location.reload();
        },
        error: function () {
            alert("Oops.. Something went wrong! :-(");
        }
    });
}

// =====================================================
// Color
// =====================================================
function colorLabelDetails(textColor, labelColor) {
    $('#color-label').css({
        "color": textColor,
        "background-color": labelColor
    });
}

function colorLabel(id, textColor, labelColor) {
    $('#color-label-' + id).css({
        "color": textColor,
        "background-color": labelColor
    });
}

function demoColors() {
    console.log("demoColors");
    var textColor = $('#color-text').val();
    var labelColor = $('#color-label').val();
    $('#text-example').css('color', textColor);
    $('#text-example').css('background-color', labelColor);
}

// =====================================================
// Actions
// =====================================================
function computerActionStartup(id) {
    var action = '/Action/PowerOn';
    $.ajax({
        type: 'GET',
        url: action,
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        data: { Id: id },
        success: function (result) {
            if (result === 'True') {
                alert("Wakeup was sent OK.");
            } else {
                alert("Wakeup was NOT SENT!");
            }
        },
        error: function () {
            alert("Error! | " + action + ' | ' + id);
        }
    });
}

function computerActionReboot(id) {
    var action = '/Action/PowerRecycle';
    $.ajax({
        type: 'GET',
        url: action,
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        data: { Id: id },
        success: function (result) {
            if (result === 'True') {
                alert("Restart was sent OK.");
            } else {
                alert("Restart was NOT SENT!");
            }
        },
        error: function () {
            alert("Error! | " + action + ' | ' + id);
        }
    });
}

function computerActionShutdown(id) {
    var action = '/Action/PowerOff';
    $.ajax({
        type: 'GET',
        url: action,
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        data: { Id: id },
        success: function (result) {
            if (result === 'True') {
                alert("Shutdown was sent OK.");
            } else {
                alert("Shutdown was NOT SENT!");
            }
        },
        error: function () {
            alert("Error! | " + action + ' | ' + id);
        }
    });
}

// =====================================================
// Tools / Network Tools
// =====================================================
function toolsPing() {
    $("#ping-data").html('Working...');

    var action = '/Action/Ping';
    var value = $("#textbox-ping").val();

    $.ajax({
        type: 'GET',
        url: action,
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        data: { hostname: value },
        success: function (result) {
            if (result === 'True') {
                $("#ping-data").html('ONLINE');
            } else {
                $("#ping-data").html('OFFLINE');
            }
        },
        error: function () {
            $("#ping-data").html('ERROR!');
        }
    });
}

// =====================================================
// Tools / Network Tools
// =====================================================
function schedulerClearLists() {
    // Target list
    $("#ScheduleTargedList").html("None!");
    // Hidden Fields
    //$("#hidden-computer-list").val("");
    //$("#hidden-color-id").val("");
    //$("#hidden-location-id").val("");
    //$("#hidden-type-id").val("");
}


function scheduleTypeChanged() {
    schedulerClearLists();
    $("#ScheduleSelectContent").html("Loading please wait...");
    var selectedType = $("#select-type").val();
    console.log("Type: " + selectedType);
    var action = "/Scheduler/SelectView/";
    $.ajax({
        type: 'GET',
        url: action,
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        data: { type: selectedType },
        success: function (result) {
            $("#ScheduleSelectContent").html(result);
        },
        error: function () {
            $("#ScheduleSelectContent").html("Oops.. Something went wrong! :-(");
        }
    });
}

$(document).ready(function scheduleOnLoad() {
    $("#ScheduleSelectContent").html("Loading please wait...");
    var selectedType = $("#select-type").val();
    console.log("Type: " + selectedType);
    var action = "/Scheduler/SelectView/";
    $.ajax({
        type: 'GET',
        url: action,
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        data: { type: selectedType },
        success: function(result) {
            $("#ScheduleSelectContent").html(result);
        },
        error: function() {
            $("#ScheduleSelectContent").html("Oops.. Something went wrong! :-(");
        }
    });
});

function scheduleSelectionChanged() {
    console.log("The selection changed!");
    // Clear the list
    $("#ScheduleTargedList").html("");

    // Create list from selected items
    var none = true;
    var computerList = "|";
    $('.checkbox-individual-select').each(function (i, obj) {
        if (obj.checked) {
            none = false;
            var id = obj.id.replace("c-", "");
            var computerName = $("#n-" + id).html();
            computerList += "," + id;
            console.log(computerList);
            $("#ScheduleTargedList").append(computerName + "<br/>");
        }
    });

    // Show if there is none.
    if (none) {
        schedulerClearLists();
    } else {
        $("#hidden-computer-list").val(computerList);
    }
}

function scheduleColorSelectChanged() {
    console.log("Color changed!");
    // Clear list
    schedulerClearLists();
    var id = "";
    var list = "";
    // Get Selected
    $(".radiobutton-color-select").each(function (i, obj) {
        if (obj.checked) {
            id = obj.id.replace("c-", "");
            console.log("ID: " + id);
            list = $("#tlist-" + id).val();
            console.log("List: " + list);
            $("#ScheduleTargedList").html(list);
        }
    });
    $("#hidden-color-id").val(id);
    if (list === "") {
        schedulerClearLists();
    }
}

function scheduleLocationSelectChanged() {
    console.log("Location changed!");
    // Clear list
    schedulerClearLists();
    var id = "";
    var list = "";
    // Get Selected
    $(".radiobutton-location-select").each(function (i, obj) {
        if (obj.checked) {
            id = obj.id.replace("c-", "");
            console.log("ID: " + id);
            list = $("#tlist-" + id).val();
            console.log("List: " + list);
            $("#ScheduleTargedList").html(list);
        }
    });
    $("#hidden-location-id").val(id);
    if (list === "") {
        schedulerClearLists();
    }
}

function scheduleTypeSelectChanged() {
    console.log("Type changed!");
    // Clear list
    schedulerClearLists();
    var id = "";
    var list = "";
    // Get Selected
    $(".radiobutton-type-select").each(function (i, obj) {
        if (obj.checked) {
            id = obj.id.replace("c-", "");
            console.log("ID: " + id);
            list = $("#tlist-" + id).val();
            console.log("List: " + list);
            $("#ScheduleTargedList").html(list);
        }
    });
    $("#hidden-type-id").val(id);
    if (list === "") {
        schedulerClearLists();
    }
}