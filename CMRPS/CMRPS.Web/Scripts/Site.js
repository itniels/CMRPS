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
// Scheduler
// =====================================================
function schedulerClearLists() {
    $("#ScheduleTargedList").html("None!");
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

function scheduleOnLoad() {
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
            $("#ScheduleTargedList").append(computerName + "<br/>");
        }
    });
    computerList += "]";
    computerList = computerList.replace("|,", "[");

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

function scheduleOnEdit() {
    console.log("On Edit exec");
    var selectedType = $("#hidden-selectedType").html();
    var currentType = $("#select-type option:selected").text();

    console.log(selectedType + " | " + currentType);

    if (selectedType === currentType) {
        console.log("Match!");
        // Individual
        if (selectedType === "Individual") {
            var list = $("#hidden-computer-list").val();
            // Check list
            $(JSON.parse(list)).each(function (i, id) {
                $("#c-" + id).prop('checked', true);
            });
            scheduleSelectionChanged();
        }
        // Color
        if (selectedType === "Color") {
            var colorId = $("#hidden-color-id").val();
            console.log("Color ID: " + colorId);
            $("#c-" + colorId).prop('checked', true);
            scheduleColorSelectChanged();
        }
        // Location
        if (selectedType === "Location") {
            var locationId = $("#hidden-location-id").val();
            $("#c-" + locationId).prop('checked', true);
            scheduleLocationSelectChanged();
        }
        // Computer Type
        if (selectedType === "Type") {
            var computertypeId = $("#hidden-type-id").val();
            $("#c-" + computertypeId).prop('checked', true);
            scheduleTypeSelectChanged();
        }
    } else {
        //schedulerClearLists();
    }
}

// =====================================================
// List View
// =====================================================
function filterListView() {
    $("#filtering-wait").show();
    // get filter values
    var name = $("#NameFilter").val().toUpperCase();
    var hostname = $("#HostnameFilter").val().toUpperCase();
    var status = $("#StatusFilter").val().toUpperCase();
    var type = $("#TypeFilter").val().toUpperCase();
    var color = $("#ColorFilter").val().toUpperCase();
    var location = $("#LocationFilter").val().toUpperCase();

    var cTotal = 0;
    var cShown = 0;

    // Filter the list
    $(".computer-row").each(function() {
        var isMatch = true;
        // get vars
        var itemId = $(this).find("#hidden-id").html().toUpperCase();
        var itemName = $(this).find("#hidden-name").html().toUpperCase();
        var itemHostname = $(this).find("#hidden-hostname").html().toUpperCase();
        var itemStatus = $(this).find("#hidden-status").html().toUpperCase();
        var itemType = $(this).find("#hidden-type").html().toUpperCase();
        var itemColor = $(this).find("#hidden-color").html().toUpperCase();
        var itemLocation = $(this).find("#hidden-location").html().toUpperCase();

        // Name Contains
        if (name !== "") {
            if (itemName.indexOf(name) === -1)
                isMatch = false;
        }

        // Hostname Contains
        if (hostname !== "") {
            if (itemHostname.indexOf(hostname) === -1)
                isMatch = false;
        }

        // Status
        if (status !== "") {
            if (itemStatus !== status)
                isMatch = false;
        }

        // Type
        if (type !== "") {
            if (itemType !== type)
                isMatch = false;
        }

        // Color
        if (color !== "") {
            if (itemColor !== color)
                isMatch = false;
        }

        // Location
        if (location !== "") {
            if (itemLocation !== location)
                isMatch = false;
        }

        cTotal++;
        // Show or Hide
        if (isMatch) {
            $("#tr-id-" + itemId).show();
            cShown++;
        } else {
            $("#tr-id-" + itemId).hide();
        }

        // Set values
        $("#filtering-count").html(cShown + " of " + cTotal);
    });

    //// Ajax
    //var action = '/View/GetFilterList';
    //$.ajax({
    //    type: 'GET',
    //    url: action,
    //    contentType: "application/json; charset=utf-8",
    //    datatype: "json",
    //    data: {
    //        name: name,
    //        hostname: hostname,
    //        status: status,
    //        type: type,
    //        color: color,
    //        location: location
    //    },
    //    success: function (result) {
    //        $("#listview-list").html(result);
    //        $("#filtering-wait").hide();
    //    },
    //    error: function () {
    //        $("#listview-list").html('Oops.. Something went wrong! :-(');
    //        $("#filtering-wait").hide();
    //    }
    //});

    $("#filtering-wait").hide();
}

function listViewClearFilters() {
    // Clear Filters
    $("#NameFilter").val("");
    $("#HostnameFilter").val("");
    $("#StatusFilter").val("");
    $("#TypeFilter").val("");
    $("#ColorFilter").val("");
    $("#LocationFilter").val("");

    // Load without filters
    filterListView();
}

// =====================================================
// SignalR
// =====================================================

// Home
function srUpdateHome() {
    // Ref the hub
    var myhub = $.connection.liveUpdatesHub;
    // Event handler(s)
    myhub.client.updateHomePage = function () {
        var action = "/Home/UpdateData/";
        $.ajax({
            type: 'GET',
            url: action,
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (result) {
                $("#homeContent").html(result);
            },
            error: function () {
                $("#homeContent").html("Oops.. Something went wrong! :-(");
            }
        });
    };
    $.connection.hub.start();

    
}

// View/Overview
function srUpdateOveriew() {
    // Ref the hub
    var myhub = $.connection.liveUpdatesHub;
    // Event handler(s)
    myhub.client.updateOverview = function(id, status, ip, mac, lastSeen) {
        if (status) {
            $("#overview-well-" + id).removeClass("well-offline");
            $("#overview-well-" + id).addClass("well-online");
        } else {
            $("#overview-well-" + id).removeClass("well-online");
            $("#overview-well-" + id).addClass("well-offline");
        }
        // IP and MAC
        $("#overview-popup-" + id).prop("title", "IP: " + ip + "\nMAC: " + mac + "\nLast Seen: " + lastSeen);

    };
    $.connection.hub.start();
}

// View/ListView
function srUpdateListView() {
    // Ref the hub
    var myhub = $.connection.liveUpdatesHub;
    // Event handler(s)
    myhub.client.updateListView = function (id, status, ip, mac, lastSeen) {
        if (status) {
            $("#tr-id-" + id).removeClass("tablerow-offline");
            $("#tr-id-" + id).addClass("tablerow-online");
            $("#tr-id-" + id).find("#hidden-status").html("Online");
        } else {
            $("#tr-id-" + id).removeClass("tablerow-online");
            $("#tr-id-" + id).addClass("tablerow-offline");
            $("#tr-id-" + id).find("#hidden-status").html("Offline");
        }
        // IP and MAC
        $("#listview-popup-" + id).prop("title", "IP: " + ip + "\nMAC: " + mac + "\nLast Seen: " + lastSeen);
        
        filterListView(); // Possibly Disruptive!
    };
    $.connection.hub.start();
}

// Manage/Computers
function srUpdateComputers() {
    // Ref the hub
    var myhub = $.connection.liveUpdatesHub;
    // Event handler(s)
    myhub.client.UpdateComputers = function (id, status, ip, mac, lastSeen) {
        if (status) {
            $("#computer-id-" + id).removeClass("label-danger");
            $("#computer-id-" + id).addClass("label-success");
            $("#computer-id-" + id).html("ONLINE");
        } else {
            $("#computer-id-" + id).removeClass("label-success");
            $("#computer-id-" + id).addClass("label-danger");
            $("#computer-id-" + id).html("OFFLINE");
        }
        // IP and MAC
        $("#computer-ip-" + id).html(ip);
        $("#computer-mac-" + id).html(mac);
    };
    $.connection.hub.start();
}

// Schedules
function srUpdateSchedules() {
    // Ref the hub
    var myhub = $.connection.liveUpdatesHub;
    // Event handler(s)
    myhub.client.UpdateSchedules = function (id, lastRun) {
        // LastRun
        $("#schedule-lastRun-" + id).html(lastRun);

    };
    $.connection.hub.start();
}
