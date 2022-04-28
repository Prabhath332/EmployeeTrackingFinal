var url = window.location.protocol + "//" + window.location.hostname + ":58145/";

$(document).ready(function () {
    if ($('.tbl').length) {

        $.getScript("/Scripts/jquery.dataTables.min.js", function () {
            $.getScript("/Scripts/dataTables.bootstrap.min.js", function () {
                
                if($("#tblHolidayDates").length)
                {
                    $('.tbl').DataTable({
                        "searching": false
                    });

                    $('#tblUserLeaves').DataTable({
                        "searching": true
                    });
                    
                }
                else
                {
                    $('.tbl').DataTable();
                }

                $(".dataTables_filter").attr("style", "text-align:right;");
            });

        });

    }
    
    if($('.date').length)
    {
        var isInOut = "";

        if ($(".futdis").length)
        {
            isInOut = "+0d";
        }

        

        $(".date").attr("autocomplete", "off");
        $.getScript("/plugins/datepicker/bootstrap-datepicker.js", function () {
            if ($("#txtUserDOB").length) {
                $('.date').datepicker();
            }
            else if($("#hfInOutUserId").length)
            {
                $('.date').datepicker({
                    endDate: isInOut,
                    autoclose: true
                });
            }
            else {
                $('.date').datepicker({
                    endDate: isInOut,
                    autoclose: true,
                    daysOfWeekDisabled: [0]
                }).on('changeDate', function (ev) {
                    if (this.id == "txtLeaveTo") {
                        validateWeekend();
                    }

                    if (this.id == "txtLeaveFrom") {
                        validateWeekend();
                    }
                });
            }
            

        });

        if($(".leavedate").length)
        {
            $.getScript("/plugins/datepicker/bootstrap-datepicker.js", function () {
                var checkin = $("#txtLeaveFrom").datepicker({
                    daysOfWeekDisabled: [0],
                    beforeShowDay: function (date) {
                        return date.valueOf();
                    },
                    autoclose: true

                }).on('changeDate', function (ev) {
                    if (ev.date.valueOf() >= checkout.datepicker("getDate").valueOf() || !checkout.datepicker("getDate").valueOf()) {

                        var newDate = new Date(ev.date);
                        newDate.setDate(newDate.getDate());
                        checkout.datepicker("update", newDate);

                    }
                    //$("#txtLeaveTo")[0].focus();
                });


                var checkout = $("#txtLeaveTo").datepicker({
                    daysOfWeekDisabled: [0],
                    beforeShowDay: function (date) {
                        if (!checkin.datepicker("getDate").valueOf()) {
                            return date.valueOf() >= new Date().valueOf();
                        } else {
                            return date.valueOf() >= checkin.datepicker("getDate").valueOf();
                        }
                    },
                    autoclose: true

                }).on('changeDate', function (ev) { });

            });           
        }

        if ($(".filterdate").length) {
            $.getScript("/plugins/datepicker/bootstrap-datepicker.js", function () {
                                
                var checkin = $("#txtDateFrom").datepicker({
                    daysOfWeekDisabled: [0],
                    beforeShowDay: function (date) {
                        return date.valueOf();
                    },
                    autoclose: true

                }).on('changeDate', function (ev) {
                    if (ev.date.valueOf() >= checkout.datepicker("getDate").valueOf() || !checkout.datepicker("getDate").valueOf()) {

                        var newDate = new Date(ev.date);
                        newDate.setDate(newDate.getDate());
                        checkout.datepicker("update", newDate);

                    }
                    //$("#txtLeaveTo")[0].focus();
                });

                var checkout = $("#txtDateTo").datepicker({
                    daysOfWeekDisabled: [0],
                    beforeShowDay: function (date) {
                        if (!checkin.datepicker("getDate").valueOf()) {
                            return date.valueOf() >= new Date().valueOf();
                        } else {
                            return date.valueOf() >= checkin.datepicker("getDate").valueOf();
                        }
                    },
                    autoclose: true

                }).on('changeDate', function (ev) { });
                

            });
        }
    }

    

    function validateWeekend()
    {        
        setTimeout(function () {
            $.get(url + "api/tracker/ValidateWeekends?userId=" + $("#hfLeaveUserId").val() + "&from=" + $("#txtLeaveFrom").val() + "&to=" + $("#txtLeaveTo").val(), function (data) {
                //var toDate = new Date($("#txtLeaveFrom").val());
                

                if ($("#ddlLeaveUnit").val() == "HALFMOR" || $("#ddlLeaveUnit").val() == "HALFEVE") {
                    $("#txtLeaveDays").val(0.5);
                    $("#hfLeaveDays").val(0.5);
                }
                else if ($("#ddlLeaveUnit").val() == "SHORT") {
                    $("#txtLeaveDays").val(0.25);
                    $("#hfLeaveDays").val(0.25);
                }
                else {
                    $("#txtLeaveDays").val(data);
                    $("#hfLeaveDays").val(data);
                    
                    if(data < 0)
                    {
                        warningMsg("You Don't Have Enough Leaves To Complete Your Request." + " Days Will be Added as No Payleave");
                    }
                }

                $(".hfLeaveType").each(function (i, elm) {
                    var remaining = this.value;
                    var noPayCount = data - remaining;
                    var leaveTypeId = this.id.replace("hfLeaveTypeName_", "");
                    
                    if ($("#ddlLeaveType").val() == leaveTypeId && $("#ddlLeaveUnit").val() != "SHORT")
                    {
                        if ($("#ddlLeaveUnit").val() == "HALFMOR" || $("#ddlLeaveUnit").val() == "HALFEVE")
                        {
                            if (remaining < 0) {
                                warningMsg("You Don't Have Enough Leaves To Complete Your Request.<br /><strong>0.5</strong> Days Will be Added as No Payleave If You Continue to Use <strong>" + $("#ddlLeaveType option:selected").text() + "</strong> Leaves");
                            }
                        }
                        else
                        {
                            if (remaining < 0) {
                                warningMsg("You Don't Have Enough Leaves To Complete Your Request.<br /><strong>" + data + "</strong> Days Will be Added as No Payleave If You Continue to Use <strong>" + $("#ddlLeaveType option:selected").text() + "</strong> Leaves");
                            }
                            else if (remaining < data) {
                                warningMsg("You Don't Have Enough Leaves To Complete Your Request.<br /><strong>" + noPayCount + "</strong> Days Will be Added as No Payleave If You Continue to Use <strong>" + $("#ddlLeaveType option:selected").text() + "</strong> Leaves");
                            }
                        }
                        
                        
                    }
                    
                });
                //warningMsg("Actual Day Count May Different from the Selected Day Count, If Your Request Include Weekends");

                //if (data == 0)
                //{
                //    warningMsg("Your Not Allowed to Apply Leave on Weekends");
                //    $("#txtLeaveTo").val("");
                //    $("#txtLeaveFrom").val("");
                //    $("#txtLeaveDays").val(0);
                //    $("#hfLeaveDays").val(0);
                //}
                //else if (data == -1)
                //{
                //    warningMsg("Your Not Allowed to Apply Leave on Sunday");
                //    $("#txtLeaveTo").val("");
                //    $("#txtLeaveFrom").val("");
                //    $("#txtLeaveDays").val(0);
                //    $("#hfLeaveDays").val(0);
                //}
                //else
                //{
                //    $("#txtLeaveDays").val(data);
                //    $("#hfLeaveDays").val(data);
                //}

            });
        }, 700);
        
    }

    if ($('.time').length) {
        $.getScript("/plugins/timepicki/timepicki.js", function () {
            $('.time').timepicki({
                
            });

        });
        
    }

    if ($("#tblAwards").length)
    {
        getAwards();
    }

    if ($("#tblPromotion").length) {
        getPromotions();
    }

    if ($("#tblExperiance").length) {
        getExperiance();
    }
    
    if($("#txtDivisionManager").length)
    {
        var options = {
            url: function (search) {
                return url + "api/tracker/SearchUser?Search=" + search;
            },

            getValue: "FirstName",
            theme: "square",
            list: {

                onSelectItemEvent: function () {
                    //var value = $("#txtNIC").getSelectedItemData().NICNo;
                    //$("#txtFullName").val($("#txtNIC").getSelectedItemData().ClientName);
                    //$("#txtPhone").val($("#txtNIC").getSelectedItemData().PhoneNo);
                    //$("#ddlSevaDivision").val($("#txtNIC").getSelectedItemData().GramaSevaDivision);
                    //$("#hfIsNew").val(value).trigger("change");
                }
            }
        };

        $("#txtDivisionManager").easyAutocomplete(options);
        $('div.easy-autocomplete').removeAttr('style');
    }

    if ($("#txtSupervisor").length) {
        var options = {
            url: function (search) {
                return url + "api/tracker/GetAboveUsers?userId=" + $("#hfLeaveUserId").val() + "&search=" + search;
            },

            getValue: "Name",
            theme: "square",
            list: {

                onSelectItemEvent: function () {
                    $("#hfLeaveSupervisorId").val($("#txtSupervisor").getSelectedItemData().UserId);
                    $("#txtSupervisor").val($("#txtSupervisor").getSelectedItemData().Name);                    
                }
            }
        };

        $("#txtSupervisor").easyAutocomplete(options);
        $('div.easy-autocomplete').removeAttr('style');
    }

    if ($(".supervisor").length) {
        var options = {
            url: function (search) {
                return url + "api/tracker/GetAboveUsers?userId=" + $(".userid").val() + "&search=" + search;
            },

            getValue: "Name",
            theme: "square",
            list: {

                onSelectItemEvent: function () {
                    $(".supervisorid").val($(".supervisor").getSelectedItemData().UserId);
                    $(".supervisor").val($(".supervisor").getSelectedItemData().Name);
                }
            }
        };

        $(".supervisor").easyAutocomplete(options);
        $('div.easy-autocomplete').removeAttr('style');
    }

    if ($(".divisionusers").length) {
        
        if ($("#hfTransferUser").val() != '0')
        {
            var options = {
                url: function (search) {
                    var divisionId;

                    if ($(".divisions").length) {
                        divisionId = $(".divisions").val();
                    }
                    else {
                        divisionId = "";
                    }

                    return url + "api/tracker/GetProjectUsers?ProjectId=" + divisionId + "&EmployeeId=" + $("#hfTransferUser").val() + "&search=" + search;
                },

                getValue: "Username",
                theme: "square",
                list: {

                    onSelectItemEvent: function () {
                        $("#hfDivisionUserId").val($(".divisionusers").getSelectedItemData().UserId);
                        $(".divisionusers").val($(".divisionusers").getSelectedItemData().Username);
                    }
                }
            };

            $(".divisionusers").easyAutocomplete(options);
            $('div.easy-autocomplete').removeAttr('style');
        }
        else
        {

        }
        

        
    }

    if ($(".cursupchange").length) {
        var options = {
            url: function (search) {
                var divisionId = $("#hfCurrentDivision").val();
                console.log("api/tracker/GetProjectUsers?ProjectId=" + divisionId + "&EmployeeId=" + $("#hfTransferUser").val() + "&search=" + search);
                //if ($(".divisions").length) {
                //    divisionId = $("#hfCurrentDivision").val();
                //}
                //else {
                //    divisionId = "";
                //}

                return url + "api/tracker/GetProjectUsers?ProjectId=" + divisionId + "&EmployeeId=" + $("#hfTransferUser").val() + "&search=" + search;
            },

            getValue: "Username",
            theme: "square",
            list: {

                onSelectItemEvent: function () {
                    $("#hfCurrentSupervisorId").val($(".cursupchange").getSelectedItemData().UserId);
                    $(".cursupchange").val($(".cursupchange").getSelectedItemData().Username);
                }
            }
        };

        $(".cursupchange").easyAutocomplete(options);
        $('div.easy-autocomplete').removeAttr('style');
    }

    if ($("#txtForwardTo").length) {
        var options = {
            url: function (search) {
                return url + "api/tracker/GetAboveUsers?userId=" + $("#hfLeaveUserId").val() + "&search=" + search;
            },

            getValue: "Name",
            theme: "square",
            list: {

                onSelectItemEvent: function () {
                    $("#hfForwardTo").val($("#txtForwardTo").getSelectedItemData().UserId);
                    $("#txtForwardTo").val($("#txtForwardTo").getSelectedItemData().Name);
                }
            }
        };

        $("#txtForwardTo").easyAutocomplete(options);
        $('div.easy-autocomplete').removeAttr('style');
    }

    //$("#btnNewTeamMember").click(function () {
    //    var id = $("#hfDivisionId").val();
    //    getProjectUsers(id);
    //    $("#TeamMemebersModal").modal("show");
    //});


    //function getProjectUsers(id) {
    //    var btnclass = '<i class="fa fa-spinner fa-pulse fa-3x fa-fw"></i>';
    //    $("#preLoader").html(btnclass);
    //    $("#divPrUsers").attr("style", "display:none;");
                
    //    console.log($('.tbl tr').length);

    //    $.get(url + "api/tracker/GetProjectUsers?ProjectId=" + id, function (data) {
    //        var tbl;

    //        $.each(data, function (index) {
    //            var userId = "'" + data[index].UserId + "'";
    //            tbl += '<tr><td>' + data[index].Username + '</td><td><button type="button" id="btnaddteamuser_' + data[index].UserId + '" onclick="addteamUser(' + userId + ');" class="btn btn-primary btn-sm"><i class="fa fa-reply">' + '</i></button></td></tr>';
    //        });

    //        $("#divPrUsers").removeAttr("style");
    //        $("#preLoader").remove();
    //        $("#projectUserstbody").empty();
    //        $("#projectUserstbody").append(tbl);


    //    }).done(function () {
    //        if ($('.tbl tr').length > 4)
    //        {
    //            $.getScript("/Scripts/jquery.dataTables.min.js", function () {
    //                $.getScript("/Scripts/dataTables.bootstrap.min.js", function () {
    //                    $('.tbl').DataTable();
    //                    $(".dataTables_filter").attr("style", "text-align:right;");
    //                });

    //            });
    //        }
            

    //        //if ($("#projectUserstbody").length)
    //        console.log($('.tbl tr').length);
    //    });

    //    console.log(5);
    //}
    
});
//////////User Profiles//////////////////////////////////
//////User Levels///////////////////////////////////////////
$("#btnNewLevel").click(function () {
    $("#hfRoleId").val(0);
    $("#txtUserLevel").val("");
    $("#divRoleLevels").attr("style", "display:none;");
    $("#UserLevelModal").modal("show");
    //getAssignedModules(0);
});

$(".btneditrole").click(function () {
    //var id = this.id.replace("btnEditRole_", "");
    //$("#divRoleLevels").removeAttr("style");

    //$("#hfRoleId").val(id);

    //$.get(url + "api/tracker/GetRole?LevelId=" + id, function (data) {
    //    $("#txtUserLevel").val(data.RoleName);
    //    $("#txtSortOrder").val(data.SortOrder);

    //    if (data.CanEdit)
    //    {
    //        $('#chkEdit').prop('checked', true);
    //    }
    //    else
    //    {
    //        $('#chkEdit').prop('checked', false);
    //    }

    //    if (data.CanViewAll) {
    //        $('#chkViewAll').prop('checked', true);
    //    }
    //    else {
    //        $('#chkViewAll').prop('checked', false);
    //    }
    //});

    //$("#UserLevelModal").modal("show");
    
    //getAssignedModules(id);
});

$("#btnSaveUserLevel").click(function () {
   
    if ($("#txtUserLevel").val() == "")
    {
        errorMsg("User Level Cannot Be Empty");
    }
    else
    {        
        $("#frmRole").submit();
    }
    
});

//function getAssignedModules(id)
//{
    

//    $.get(url + "api/tracker/GetModules?LevelId=" + id, function (data) {
        
//        $("#tbodyModules").empty();
//        $.each(data, function (index) {
//            if (data[index].IsChecked) {
//                var dataId = "'" + data[index].Id + "'";
//                $("#tbodyModules").append('<tr><td><button id="btndellevels_' + data[index].Id + '" type="button" onclick="changeLevelModal(' + dataId + ');" class="btn btn-sm btn-icon btn-danger" style="margin-right: 5px;"><i id="ispin_' + data[index].Id + '" class=" fa fa-remove changestate"></i></button></td><td>' + data[index].Module + '</td></tr>');
//            }
//            else {
//                var dataId = "'" + data[index].Id + "'";
//                $("#tbodyModules").append('<tr><td><button id="btnassignedlevels_' + data[index].Id + '" type="button" onclick="changeLevelModal(' + dataId + ');" class="btn btn-sm btn-icon btn-primary" style="margin-right: 5px;"><i id="ispin_' + data[index].Id + '" class="fa fa-plus changestate"></i></button></td><td>' + data[index].Module + '</td></tr>');
//            }

//        });

        

//    });
    
//}

function changeLevelModal(id, btnClass) {
    $("#ispin_" + id).addClass("fa-spinner fa-pulse fa-fw");
    
    $.post(url + "api/tracker/ChangeModule?RoleId=" + $("#hfRoleId").val() + "&Module=" + id, function (data) {
        
        location.reload();
    });
}

$('#UserLevelModal').on('hidden.bs.modal', function () {
    $("#txtUserLevel").val("");
    $("#txtSortOrder").val("");
});
////////////////////////////////////////////////////////////////
$("#btnEditUserProfile").click(function () {    
    $("#UserProfileModal").modal("show");
});

$("#btnNewUserBatch").click(function () {
    $("#UserFileModal").modal("show");
});

$("#btnSaveUserProfile").click(function () {
    if ($(".fullname").val() == '')
    {
        errorMsg("Full User Name is Required");
    }
    else if ($("#txtEPFNo").val() == '')
    {
        errorMsg("EPF Number is Required");
    }
    else
    {
        $("#frmUserProfile").submit();
    }
    
});

$("#btnNewUser").click(function () {
    $("#NewUserModal").modal("show");
});

$("#btnSaveUser").click(function () {
    var annual = $("#txtAllocatedAnnual").val();
    var cassual = $("#txtAllocatedCassual").val();

    //var regex = /^[0-9]+$/;
    var regex = /^[0-9]+\.[0-9]$/;

    if($("#txtEpfNo").val() == "")
    {
        errorMsg("EPF No Cannot Be Empty");
    }
    else if ($("#txtFirstName").val() == "")
    {
        errorMsg("First Name Cannot Be Empty");
    }
    else if ($("#txtAllocatedAnnual").val() == "") {
        errorMsg("Annual Leaves Cannot Be Empty");
    }
    else if ($("#txtAllocatedCassual").val() == "") {
        errorMsg("Cassual Leaves Cannot Be Empty");
    }
    //else if (!annual.match(regex)) {
    //    errorMsg("Annual Leaves Cannot Be Empty");
    //}
    //else if (!cassual.match(regex)) {
    //    errorMsg("Cassual Leaves Cannot Be Empty");
    //}
    else
    {
        //alert();
        $("#frmNewUser").submit();
    }
});

$(".editprofileimage").click(function () {
    var id = this.id.replace("userImg_", "");
    $("#UserImageModal").modal("show");
});

$(".deluser").click(function () {
    var id = this.id.replace("btnDelUser_", "");
    $("#hfDelUser").val(id);
    $("#UserDelModal").modal("show");
});

$("#btnChangePassword").click(function () {
    $("#newPsswordModal").modal("show");
});

$("#btnSavePassword").click(function () {
    if ($("#txtCurrentPassword").val() == "") {
        errorMsg("Current Password is Required");
    }
    else if ($("#txtNewPassword").val() == "") {
        errorMsg("New Password is Required");
    }
    else {
        $("#frmChangePassword").submit();
    }
});

$("#btnPassword").click(function () {
    $("#ResetPasswordModal").modal("show");
});
//////////////////////////////////Contact Info//////////////////////////////////
$("#btnSaveContactInfo").click(function () {
    if (isValidEmail($(".emailadress").val()))
    {
        $("#frmContactInfo").submit();
    }
    else
    {
        errorMsg("Please Correct Email Format");
    }
});
//////////////////////////Validations///////////////////////////////////////////
function isValidEmail(emailAddress) {
    var pattern = /^([a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+(\.[a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+)*|"((([ \t]*\r\n)?[ \t]+)?([\x01-\x08\x0b\x0c\x0e-\x1f\x7f\x21\x23-\x5b\x5d-\x7e\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|\\[\x01-\x09\x0b\x0c\x0d-\x7f\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))*(([ \t]*\r\n)?[ \t]+)?")@(([a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.)+([a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.?$/i;
    return pattern.test(emailAddress);
};
///////////////////////////////////Awards////////////////////////////////////
$("#btnAward").click(function () {
    $("#UserAwardsModal").modal("show");
});

function getAwards()
{
    $.get(url + "api/tracker/GetAwards?UserId=" + $("#hfUserId").val(), function (data) {
        $("#tbodyAwards").html();
        var tbl;
        var canEdit = $("#hfCanEdit").val();
        $.each(data, function (index) {
            var date = data[index].Date.split('T')[0];
            if (canEdit)
            {
                tbl += '<tr><td>' + data[index].Name + '</td><td>' + date + '</td><td>' + data[index].Description + '</td><td><button id="btneditawards_' + data[index].Id + '" onclick="editAwards(' + data[index].Id + ');" class="btn btn-primary btn-sm"><i class="fa fa-pencil">' + '</i></button></td></tr>';
            }
            else
            {
                tbl += '<tr><td>' + data[index].Name + '</td><td>' + date + '</td><td>' + data[index].Description + '</td><td></td></tr>';
            }
            
        });
        $("#tbodyAwards").html(tbl);
    });
}

$("#btnSaveAward").click(function () {

    var awards;
    
    $(".saveaward").removeClass("fa-save");
    $(".saveaward").addClass("fa-spinner fa-pulse fa-fw");
    $(this).addClass("disabled");
    if ($("#hfAwardId").val() == 0)
    {
        awards = { UserId: $("#hfUserId").val(), Name: $("#txtUserAward").val(), Date: $("#txtAwardDate").val(), Description: $("#txtAwardDescription").val() };
    }
    else
    {
        awards = {Id: $("#hfAwardId").val(), UserId: $("#hfUserId").val(), Name: $("#txtUserAward").val(), Date: $("#txtAwardDate").val(), Description: $("#txtAwardDescription").val() };
    }

    $.post(url + "api/tracker/AddAwards", awards, function (data) {
        if(data)
        {
            $("#UserAwardsModal").modal("hide");
            getAwards();
            successMsg("Employee Award Data Saved");

            $(".saveaward").removeClass("fa-spinner fa-pulse fa-fw");
            $(".saveaward").addClass("fa-save");

        }
        else
        {
            errorMsg("Data Could Not be Saved");

            $(".saveapromo").removeClass("fa-spinner fa-pulse fa-fw");
            $(".saveapromo").addClass("fa-save");
        }
    });

});

function editAwards(id)
{
    $.get(url + "api/tracker/GetAward?AwardId=" + id, function (data) {
        $("#txtUserAward").val(data.Name);
        $("#txtAwardDate").val(data.Date.split('T')[0]);
        $("#txtAwardDescription").val(data.Description);
        $("#hfAwardId").val(id);
        $("#UserAwardsModal").modal("show");
    });
    
}

$('#UserAwardsModal').on('hidden.bs.modal', function () {
    $("#txtUserAward").val("");
    $("#txtAwardDate").val("");
    $("#txtAwardDescription").val("");
    $("#hfAwardId").val("0");
    $("#btnSaveAward").removeClass("disabled");
});
////////////////////////////////Promotions////////////////////////////////////////////////
$("#btnPromotions").click(function () {
    $("#UserPromotionsModal").modal("show");
});

function getPromotions() {
    $.get(url + "api/tracker/GetPromotions?UserId=" + $("#hfUserId").val(), function (data) {
        $("#tbodyPromotions").html();
        var tbl;
        var canEdit = $("#hfCanEdit").val();

        $.each(data, function (index) {
            var datefrom = data[index].DateFrom.split('T')[0];
            var dateto = data[index].DateTo.split('T')[0];

            if (canEdit)
            {
                tbl += '<tr><td>' + data[index].Designation + '</td><td>' + datefrom + '</td><td>' + dateto + '</td><td>' + data[index].Division + '</td><td>' + data[index].Location + '</td><td>' + data[index].Remarks + '</td><td><button id="btneditpromo_' + data[index].Id + '" onclick="editPromotion(' + data[index].Id + ');" class="btn btn-primary btn-sm"><i class="fa fa-pencil">' + '</i></button></td></tr>';
            }
            else
            {
                tbl += '<tr><td>' + data[index].Designation + '</td><td>' + datefrom + '</td><td>' + dateto + '</td><td>' + data[index].Division + '</td><td>' + data[index].Location + '</td><td>' + data[index].Remarks + '</td><td></td></tr>';
            }
            
        });
        $("#tbodyPromotions").html(tbl);
    });
}

$("#btnSavePromotions").click(function () {

    if ($("#txtPromoDesignation").val() == "")
    {
        errorMsg("Designation is Required");
    }
    else if ($("#txtPromoDateFrom").val() == "")
    {
        errorMsg("Date From is Required");
    }
    else if ($("#txtPromoDateTo").val() == "") {
        errorMsg("Date To is Required");
    }
    else
    {
        var promotions;
        var btnclass = '<i class="fa fa-spinner fa-pulse fa-3x fa-fw"></i>';
        $(".saveapromo").removeClass("fa-save");
        $(".saveapromo").addClass("fa-spinner fa-pulse fa-fw");
        $(this).addClass("disabled");

        if ($("#hfPromotionsId").val() == 0) {
            promotions = { UserId: $("#hfUserId").val(), Designation: $("#txtPromoDesignation").val(), DateFrom: $("#txtPromoDateFrom").val(), DateTo: $("#txtPromoDateTo").val(), Division: $("#ddlPromoDivision").val(), Location: $("#txtPromoLocation").val(), Remarks: $("#txtpromoRemarks").val() };
        }
        else {
            promotions = { Id: $("#hfPromotionsId").val(), UserId: $("#hfUserId").val(), Designation: $("#txtPromoDesignation").val(), DateFrom: $("#txtPromoDateFrom").val(), DateTo: $("#txtPromoDateTo").val(), Division: $("#ddlPromoDivision").val(), Location: $("#txtPromoLocation").val(), Remarks: $("#txtpromoRemarks").val() };
        }

        $.post(url + "api/tracker/AddPromotions", promotions, function (data) {
            if (data) {
                $("#UserPromotionsModal").modal("hide");
                getPromotions();
                successMsg("Employee Promotion Saved");

                $(".saveapromo").removeClass("fa-spinner fa-pulse fa-fw");
                $(".saveapromo").addClass("fa-save");

            }
            else {
                errorMsg("Data Could Not be Saved");

                $(".saveapromo").removeClass("fa-spinner fa-pulse fa-fw");
                $(".saveapromo").addClass("fa-save");
            }
        });
    }
    

});

function editPromotion(id) {
    $.get(url + "api/tracker/GetPromotion?PromotionId=" + id, function (data) {
        $("#txtPromoDesignation").val(data.Designation);
        $("#txtPromoDateFrom").val(data.DateFrom.split('T')[0]);
        $("#txtPromoDateTo").val(data.DateTo.split('T')[0]);
        $("#ddlPromoDivision").val(data.Division);
        $("#txtPromoLocation").val(data.Location);
        $("#txtpromoRemarks").val(data.Remarks);
        $("#hfPromotionsId").val(id);
        $("#UserPromotionsModal").modal("show");
    });

}

$('#UserPromotionsModal').on('hidden.bs.modal', function () {
    $("#txtPromoDesignation").val("");
    $("#txtPromoDateFrom").val("");
    $("#txtPromoDateTo").val("");
    $("#txtPromoDivision").val("");
    $("#txtPromoLocation").val("");
    $("#txtpromoRemarks").val("");
    $("#hfPromotionsId").val("0");
    $("#btnSavePromotions").removeClass("disabled");
});

///////////////////////////////////////////////////Experiances//////////////////////////////////////////

$("#btnExperiance").click(function () {
    $("#UserExperianceModal").modal("show");
});

function getExperiance() {
    $.get(url + "api/tracker/GetExperiances?UserId=" + $("#hfUserId").val(), function (data) {
        $("#tbodExperiance").html();
        var tbl;
        var canEdit = $("#hfCanEdit").val();
        $.each(data, function (index) {
            var datefrom = data[index].DateFrom.split('T')[0];
            var dateto = data[index].DateTo.split('T')[0];

            if (canEdit)
            {
                tbl += '<tr><td>' + data[index].Organization + '</td><td>' + data[index].Designation + '</td><td>' + datefrom + '</td><td>' + dateto + '</td><td><button id="btneditexp_' + data[index].Id + '" onclick="editExperiance(' + data[index].Id + ');" class="btn btn-primary btn-sm"><i class="fa fa-pencil">' + '</i></button></td></tr>';
            }
            else
            {
                tbl += '<tr><td>' + data[index].Organization + '</td><td>' + datefrom + '</td><td>' + dateto + '</td><td></td></tr>';
            }
            
        });
        $("#tbodExperiance").html(tbl);
    });
}

$("#btnSaveExperiance").click(function () {

    var experiance;
    var btnclass = '<i class="fa fa-spinner fa-pulse fa-3x fa-fw"></i>';
    $(".saveaexe").removeClass("fa-save");
    $(".saveaexe").addClass("fa-spinner fa-pulse fa-fw");
    $(this).addClass("disabled");

    if ($("#hfExperianceId").val() == 0) {
        experiance = { UserId: $("#hfUserId").val(), Organization: $("#txtExOrganization").val(), DateFrom: $("#txtExDateFrom").val(), DateTo: $("#txtExDateTo").val() };
    }
    else {
        experiance = { Id: $("#hfExperianceId").val(), UserId: $("#hfUserId").val(), Organization: $("#txtExOrganization").val(), DateFrom: $("#txtExDateFrom").val(), DateTo: $("#txtExDateTo").val() };
    }

    $.post(url + "api/tracker/AddExperiance", experiance, function (data) {
        if (data) {
            $("#UserExperianceModal").modal("hide");
            getExperiance();
            successMsg("Employee Experiances Saved");

            $(".saveaexe").removeClass("fa-spinner fa-pulse fa-fw");
            $(".saveaexe").addClass("fa-save");

        }
        else {
            errorMsg("Data Could Not be Saved");

            $(".saveaexe").removeClass("fa-spinner fa-pulse fa-fw");
            $(".saveaexe").addClass("fa-save");
        }
    });

});

function editExperiance(id) {
    $.get(url + "api/tracker/GetExperiance?ExperianceId=" + id, function (data) {
        $("#txtExOrganization").val(data.Organization);
        $("#txtExDateFrom").val(data.DateFrom.split('T')[0]);
        $("#txtExDateTo").val(data.DateTo.split('T')[0]);        
        $("#hfExperianceId").val(id);
        $("#UserExperianceModal").modal("show");
    });

}

$('#UserExperianceModal').on('hidden.bs.modal', function () {
    $("#txtExOrganization").val("");
    $("#txtExDateFrom").val("");
    $("#txtExDateTo").val("");   
    $("#hfExperianceId").val("0");
    $("#btnSaveExperiance").removeClass("disabled");
});
//////////////////////////////Company Manager///////////////////////////////////////////
$("#btnNewCompany").click(function () {
    $("#CompaniesModal").modal("show");
});

$("#btnSaveCompany").click(function () {

    if ($("#txtCompanyName").val() == "")
    {
        errorMsg("Company Name is Required");
    }
    else if ($("#txtCompanyPhone").val() == "") {
        errorMsg("Please Enter a Phone Number");
    }
    else if ($.isNumeric($("#txtCompanyPhone").val()) == false)
    {
        errorMsg("Please Enter a Valid Phone Number");
    }
    else if ($.isNumeric($("#txtCompanyFax").val())) {
        errorMsg("Please Enter a Valid Fax Number");
    }
    else if ($("#txtCompanyEmail").val() == "") {
        errorMsg("Please Enter a Email Address");
    }
    else if (isValidEmail($("#txtCompanyEmail").val()) == false) {
        errorMsg("Please Enter a Valid Email Address");
    }
    else
    {
        $("#frmCompany").submit();
        
    }
    
});

$(".btneditcompany").click(function () {
    var id = this.id.replace("btnEditCompany_", "");

    $.get(url + "api/tracker/GetCompany?CompanyId=" + id, function (data) {
        $("#txtCompanyName").val(data.CompanyName);
        $("#txtCompanyPhone").val(data.Phone);
        $("#txtCompanyEmail").val(data.Email);
        $("#txtCompanyFax").val(data.Fax);
        $("#hfCompanyId").val(data.Id);

        $("#CompaniesModal").modal("show");
    });
});

$(".btndeletecompany").click(function () {

    var id = this.id.replace("btnDelCompany_", "");
    $("#hfDelCompany").val(id);
    $("#CompanyDelModal").modal("show");
});

$('#CompaniesModal').on('hidden.bs.modal', function () {
    $("#txtCompanyName").val("");
    $("#txtCompanyPhone").val("");
    $("#txtCompanyEmail").val("");
    $("#txtCompanyFax").val("");
    $("#hfCompanyId").val(0);
});

/////////////////////////////////Division Management///////////////////////////////
$("#btnNewDivision").click(function () {
    $("#DivisionsModal").modal("show");
});

$(".btndeletedivision").click(function () {
    var id = this.id.replace("btnDeleteDivision_", "");
    $("#hfDelDivision").val(id);
    $("#DivisionDelModal").modal("show");
});

$("#btnSaveDivision").click(function () {
    if ($("#txtDivisionName").val() == "") {
        errorMsg("Division Name is Required");
    }
    else if ($("#txtDivisionManager").val() == "") {
        errorMsg("Project Manager Name is Required");
    }
    else {
        $("#frmDivision").submit();
    }
    
});

$(".btneditdivision").click(function () {
    var id = this.id.replace("btnEditDivision_", "");

    $.get(url + "api/tracker/GetDivision?DivisionId=" + id, function (data) {
        $("#txtDivisionName").val(data.ProjectName);
        $("#txtDivisionDescription").val(data.Description);
        $("#txtDivisionManager").val(data.ProjectManager);
        $("#ddlCompanies").val(data.CompanyId);
        $("#hfDivisionId").val(data.Id);

        $("#DivisionsModal").modal("show");
    });
});

$('#DivisionsModal').on('hidden.bs.modal', function () {
    $("#txtDivisionName").val("");
    $("#txtDivisionDescription").val("");
    $("#txtDivisionManager").val("");
    $("#hfDivisionId").val(0);
});


////////////////////////////////////Team Management/////////////////////////////////

$("#btnNewTeam").click(function () {    
    $("#TeamsModal").modal("show");
});

$("#btnSaveTeam").click(function () {
    $("#frmTeam").submit();
});

$("#ddlTeamDivision").change(function () {
    $("#hfTeamId").val(this.value);
});

$(".btndelteam").click(function () {
    var id = this.id.replace("btnDelTeam_", "");
    $("#hfDelTeam").val(id);
    $("#TeamDelModal").modal("show");
});

$(".btneditteam").click(function () {
    var id = this.id.replace("btnEditTeam_", "");

    $.get(url + "api/tracker/GetTeam?TeamId=" + id, function (data) {
        $("#txtTeamName").val(data.TeamName);
        $("#txtTeamLocation").val(data.Location);        
        $("#hfTeamId").val(data.Id);
        $("#ddlTeamDivision").val(data.ProjectId);
        $("#TeamsModal").modal("show");
    });
});

$('#TeamsModal').on('hidden.bs.modal', function () {
    $("#txtTeamName").val("");
    $("#txtTeamLocation").val("");    
    $("#hfTeamId").val(0);
});

////////////////////////////////Team Members//////////////////////////////////////////

$("#btnNewTeamMember").click(function () {
    var id = $("#hfDivisionId").val();    
    getProjectUsers(id);
    $("#TeamMemebersModal").modal("show");
});

$('#TeamMemebersModal').on('hidden.bs.modal', function () {    
    
});

function getProjectUsers(id)
{
    var btnclass = '<i class="fa fa-spinner fa-pulse fa-3x fa-fw"></i>';
    $("#preLoader").html(btnclass);
    $("#divPrUsers").attr("style", "display:none;");
    console.log($("#projectUserstbody").length);
    $.get(url + "api/tracker/GetProjectUsers?ProjectId=" + id, function (data) {        
        var tbl;

        $.each(data, function (index) {
            var userId = "'" + data[index].UserId + "'";
            tbl += '<tr><td>' + data[index].Username + '</td><td><button type="button" id="btnaddteamuser_' + data[index].UserId + '" onclick="addteamUser(' + userId + ');" class="btn btn-primary btn-sm"><i class="fa fa-reply">' + '</i></button></td></tr>';
        });

        $("#divPrUsers").removeAttr("style");
        $("#preLoader").remove();
        $("#projectUserstbody").empty();
        $("#projectUserstbody").append(tbl);

        
    }).done(function () {
        //$.getScript("/Scripts/jquery.dataTables.min.js", function () {
        //    $.getScript("/Scripts/dataTables.bootstrap.min.js", function () {
        //        $('.tbl').DataTable();
        //        $(".dataTables_filter").attr("style", "text-align:right;");
        //    });

        //});

        ////if ($("#projectUserstbody").length)
        //console.log($("#projectUserstbody").length);
    });

    console.log(5);
}

function addteamUser(id)
{
    $("#hfUserId").val(id);
    $(".frmTeamMembers").submit();
}

$("#btnPost").click(function () {
    $.post(url + "api/tracker/MobileLogin?Username=chayed&Password=fukuash0le", function (data) {
        console.log(data);
    });
});

////////////////////////////////Leave Management//////////////////////////////////////////
function doUndo() {
    document.execCommand('undo', false, null);
}

$("#btnNewType").click(function () {
    $("#NewLeaveTypeModal").modal("show");
});

$("#btnLeaveRpt").click(function () {
    $("#LeaveReportModal").modal("show");
});

$(".btneditleavetype").click(function () {
    var id = this.id.replace("btnEditLeaveType_", "");
    $("#hfLeaveId").val(id);

    $.get(url + "api/tracker/GetLeaveType?LeaveId=" + id, function (data) {
        $("#txtType").val(data.LeaveType1);
        $("#NewLeaveTypeModal").modal("show");
    });    
    
});

$("#btnSaveType").click(function () {
    if ($("#txtType").val() == "")
    {
        errorMsg("Leave Type Name is Required");
    }
    else
    {
        $("#frmLeaveType").submit();
    }
});

//$("#btnFilter").click(function () {
//    if ($("#Division").val() == "0") {
//        errorMsg("Leave Type Name is Required");
//    }
//    else {
//        $("#frmLeaveType").submit();
//    }
//});

$("#btnSaveAllocation").click(function () {
    if ($(".leavetypecount").val() == "") {
        errorMsg("Leave Count is Required");
        $(this).val(0);
    }
    else {
        $("#frmLeaveAllocation").submit();
    }
});

$("#btnAllocation").click(function () {
    $("#LeaveAllocationModal").modal("show");
});

$("#btnLeaveUpload").click(function () {
    $("#AllocateUserLeavesModal").modal("show");
});

$(".btneditleavecount").click(function () {
    var id = this.id.replace("btnEditLeaveCount_", "");

    $.get(url + "api/tracker/GetLevelLeave?UserRole=" + id, function (data) {
        
        $.each(data, function (index) {
            var leaveId = "#LeaveType_" + data[index].LeaveType;
            $(leaveId).val(data[index].LeaveCount);
        });

        $("#ddlUserRoles").val(data[0].UserLevelId);
        $("#hfAllocatedRoleId").val(data[0].UserLevelId);
    });

    $("#LeaveAllocationModal").modal("show");
});

$(".btnedituserleave").click(function () {
    var id = this.id.replace("btnEditUserLeave_", "");
    var year = $("#hfLeaveYear").val();

    $.get(url + "api/tracker/GetUsersLeave?UserId=" + id + "&Year=" + year, function (data) {
        //console.log(data);

        $.each(data, function (index) {
            var leaveAllocated = "#LeaveType_Allocated_" + data[index].LeaveId;
            var leaveRemaining = "#LeaveType_Remaining_" + data[index].LeaveId;

            $(leaveAllocated).val(data[index].AllocatedCount);
            $(leaveRemaining).val(data[index].RemainingCount);
        });

        //console.log(data);
        $("#hfEditUser").val(id);
    });

    $("#EditUserLeavesModal").modal("show");
});

$(".leavetypeallocated").on("input propertychange paste", function () {
    var id = this.id.replace("LeaveType_Allocated_", "#LeaveType_Remaining_");
    var remaining = parseFloat($(id).val());
    var allocated = parseFloat(this.value);
   
    if (allocated.toString() === "NaN")
    {
        doUndo();
        errorMsg("Numaric Value Required");
    }
    else
    {
        if (parseFloat(remaining) > parseFloat(allocated)) {
            doUndo();
            errorMsg("Allocated Count Could not be Less than Remaining");
        }
    }
        
});

$("#ddlUserRoles").change(function () {
    $("#hfAllocatedRoleId").val(this.value);
});

$("#ddlLeaveType").change(function () {
    var txt = $("#ddlLeaveType").val();

    if (txt == 3012 || txt == 3011 || txt == 3010)
    {
        warningMsg("Supporting Documents are Required");
    }
    
    
});

$("#LeaveAllocationModal").on('hidden.bs.modal', function () {
    $(".leavetypecount").val("");
    $("#ddlUserRoles").val(0);
});

$("#ddlLeaveUnit").change(function () {
    if (this.value == "HALFMOR" || this.value == "HALFEVE")
    {
        $("#txtLeaveTo").attr("disabled", "true");
        $("#ddlLeaveType").removeAttr("disabled", "false");
        $("#txtLeaveDays").val(0.5);
    }
    else if (this.value == "SHORT")
    {
        $("#ddlLeaveType").attr("disabled", "true");
        $("#ddlLeaveType").val(0);
        $("#txtLeaveTo").attr("disabled", "true");
        $("#txtLeaveDays").val(0);
    }
    else
    {
        $("#txtLeaveDays").val("");
        $("#ddlLeaveType").removeAttr("disabled", "false");
        $("#txtLeaveTo").removeAttr("disabled", "false");
    }
});

$("#btnRequestLeave").click(function () {

    if ($("#ddlLeaveUnit").val() == "FULL") {
        if ($("#txtLeaveFrom").val() == "") {
            errorMsg("From Date is Required");
        }
        else if ($("#txtLeaveTo").val() == "") {
            errorMsg("To Date is Required");
        }
        else if ($("#hfLeaveSupervisorId").val() == "") {
            errorMsg("Supervisor is Required");
        }
        else if ($("#txtReason").val() == "") {
            errorMsg("Reason For Leave is Required");
        }
        else {
            
            $.get(url + "api/tracker/CheckUserLeaves?userId=" + $("#hfLeaveUserId").val() + "&leaveType=" + $("#ddlLeaveType").val() + "&unit=" + $("#ddlLeaveUnit").val() + "&days=" + $("#txtLeaveDays").val() + "&date=" + $("#txtLeaveFrom").val(), function (data) {

                if (data != 0) {
                    errorMsg(data);
                }
                else {
                    $("#frmSaveLeave").submit();                    
                }

            });
        }
    }
    else if ($("#ddlLeaveUnit").val() == "SHORT")
    {
        if ($("#txtLeaveFrom").val() == "") {
            errorMsg("From Date is Required");
        }
        else if ($("#hfLeaveSupervisorId").val() == "") {
            errorMsg("Supervisor is Required");
        }
        else if ($("#txtReason").val() == "") {
            errorMsg("Reason For Leave is Required");
        }
        else {
            $.get(url + "api/tracker/CheckUserLeaves?userId=" + $("#hfLeaveUserId").val() + "&leaveType=0&unit=" + $("#ddlLeaveUnit").val() + "&days=" + $("#txtLeaveDays").val() + "&date=" + $("#txtLeaveFrom").val(), function (data) {

                if (data != 0) {
                    errorMsg(data);
                }
                else {
                    $("#frmSaveLeave").submit();
                }

            });
        }
    }
    else {
        if ($("#txtLeaveFrom").val() == "") {
            errorMsg("From Date is Required");
        }
        else if ($("#hfLeaveSupervisorId").val() == "") {
            errorMsg("Supervisor is Required");
        }
        else if ($("#txtReason").val() == "") {
            errorMsg("Reason For Leave is Required");
        }
        else {
            $.get(url + "api/tracker/CheckUserLeaves?userId=" + $("#hfLeaveUserId").val() + "&leaveType=" + $("#ddlLeaveType").val() + "&unit=" + $("#ddlLeaveUnit").val() + "&days=" + $("#txtLeaveDays").val() + "&date=" + $("#txtLeaveFrom").val(), function (data) {

                if (data != 0) {
                    errorMsg(data);
                }
                else {
                    $("#frmSaveLeave").submit();
                }

            });
        }
    }

});

$("#btnEnforceLeave").click(function () {
    if ($("#ddlLeaveUnit").val() != "SHORT") {
        if ($("#txtLeaveFrom").val() == "") {
            errorMsg("From Date is Required");
        }
        else if ($("#txtLeaveTo").val() == "") {
            errorMsg("To Date is Required");
        }
        else if ($("#txtReason").val() == "") {
            errorMsg("Reason For Leave is Required");
        }
        else if ($("#ddlLeaveType").val() == "0") {

            errorMsg("Select a Leave Type");
        }
        else {
            //alert("OH");
            $("#frmEnforceUserLeave").submit();
        }
    }
    else {
        if ($("#txtLeaveFrom").val() == "") {
            errorMsg("From Date is Required");
        }
        else if ($("#txtLeaveTo").val() == "") {
            errorMsg("To Date is Required");
        }
        else if ($("#txtReason").val() == "") {
            errorMsg("Reason For Leave is Required");
        }
        else {
            //alert("OH");
            $("#frmEnforceUserLeave").submit();
        }
    }
    
});

$(".btnreject").click(function () {
    var id = this.id.replace("btnLeaveCancel_", "");

    $("#hfIsApprove").val(0);
    $("#hfLeaveId").val(id);

    $("#RejectLeaveModal").modal("show");
});

$("#btnRejectLeave").click(function () {
    if ($("#txtRemarks").val() == "") {
        errorMsg("Please Enter a Reason to Cancel Leave");
    }
    else {
        $("#frmApproveLeave").submit();
    }
    
    
});

$(".btnaccept").click(function () {
    var id = this.id.replace("btnLeaveAccept_", "");

    $("#hfIsApprove").val(1);
    $("#hfLeaveId").val(id);

    $("#frmApproveLeave").submit();
});

$(".btnforwardleave").click(function () {
    var id = this.id.replace("btnLeaveForward_", "");
    
    $("#hfForwardLeaveId").val(id);

    $("#ForwardLeaveModal").modal("show");
});

$("#btnForwardLeave").click(function () {
    $("#frmForwardLeave").submit();
});

$("#ForwardLeaveModal").on('hidden.bs.modal', function () {
    $("#hfForwardLeaveId").val(0);
    $("#hfForwardTo").val(0);
    $("#txtForwardTo").val('');
});

$(".usercancel").click(function () {
    var id = this.id.replace("btnLeaveCancel_", "");

    $("#hfCancelLeaveId").val(id);

    $("#UserCancelLeaveModal").modal("show");
});

$(".btncancelleave").click(function () {
    var id = this.id.replace("btnDeleteLeaveType_", "");

    $("#hfLeaveId").val(id);
    //$("#hfEmployeeId").val();
    $("#RejectLeaveModal").modal("show");
});

function calculateLeaveDays()
{
    //if ($("#frmEnforceUserLeave").length)
    //{
    //    var oneDay = 24 * 60 * 60 * 1000;
    //    var firstDate = new Date($("#txtLeaveFrom").val());
    //    var secondDate = new Date($("#txtLeaveTo").val());

    //    var diffDays = Math.round(Math.abs((firstDate.getTime() - secondDate.getTime()) / (oneDay)));
    //    $("#txtLeaveDays").val(diffDays + 1);
    //    $("#hfLeaveDays").val(diffDays + 1);
    //}
    //else
    //{
    //    if ($("#ddlLeaveUnit").val() == "FULL") {
    //        var oneDay = 24 * 60 * 60 * 1000;
    //        var firstDate = new Date($("#txtLeaveFrom").val());
    //        var secondDate = new Date($("#txtLeaveTo").val());

    //        var diffDays = Math.round(Math.abs((firstDate.getTime() - secondDate.getTime()) / (oneDay)));
    //        $("#txtLeaveDays").val(diffDays + 1);
    //        $("#hfLeaveDays").val(diffDays + 1);
    //    }
    //}
    
    
}

///////////////////In Out Corrections////////////////////////////
$("#btnRequestInOut").click(function () {
    if ($(".form-control").val() == "") {
        errorMsg("All The Fields are Required");        
    }
    else {
        $("#frmCorrectInOut").submit();
    }
});

$(".btnacceptinout").click(function () {
    var id = this.id.replace("btnInOutAccept_", "");

    $("#hfInOutApprove").val(1);
    $("#hfInOutId").val(id);
   
    $("#frmApproveInOut").submit();
});

$(".btnrejectinout").click(function () {
    var id = this.id.replace("btnInOutCancel_", "");

    $("#hfInOutApprove").val(0);
    $("#hfInOutId").val(id);

    $("#RejectInOutModal").modal("show");
});

$("#btnRejectInOut").click(function () {
    $("#frmApproveInOut").submit();
});

$(".btncancelinout").click(function () {
    var id = this.id.replace("btnCacelInOut_", "");

    $("#hfInOutApprove").val(-1);
    $("#hfInOutId").val(id);

    $("#RejectInOutModal").modal("show");
});

$("#btnCancelInOut").click(function () {
    $("#frmApproveInOut").submit();
});

$(".btnattendancehistory").click(function () {
    var id = this.id.replace("btnDeleteAttendance_", "");

    $("#hfAttendanceId").val(id);
    $("#RejectAttendanceModal").modal("show");
});

$("#btnDeleteAttendance").click(function () {
    if ($("#txtRemarks").val() == "") {
        errorMsg("Please Enter a Reason to Delete In/Out Records");
    }
    else {
        $("#frmDeleteAttendance").submit();
    }

});
////////////////////////////News///////////////////////////////////////
$(".newslike").click(function () {
    var id = this.id.replace("like_", "");
    var count = $("#likecount_" + id).text().split(' ')[0];
    $.post(url + "api/tracker/Like?UserId=" + $("#hfUserId").val() + "&NewsId=" + id, function (data) {
        var newCount = parseInt(count) + 1;

        if (newCount == 1)
        {
            successMsg("You like this");
            $("#likecount_" + id).text("You like this");
        }
        else if (newCount > 1)
        {
            successMsg("You and " + count + " Others likes This");
            $("#likecount_" + id).text("You and " + count + " Others likes This");            
        }

        $("#like_" + id).addClass("ctrl-hide");
           
    });    
});

$("#btnSaveNews").click(function () {
    if ($("#txtTitle").val() == "")
    {
        errorMsg("Title is Required");
    }
    else
    {
        $("#frmNews").submit();
    }
    
});

///////////////////////////////Employee Transfers////////////////////////
$("#btnSearchEmp").click(function () {
    $.get(url + "api/tracker/GetEmployeesBelow?search=" + $("#txtEmpNo").val() + "&userId=" + $("#hfUserId").val(), function (data) {
        if ($("#frmEnforceUserLeave").length)
        {
            $("#txtEmpName").val(data.FirstName);
            $("#txtDesignation").val(data.Designation);
            $("#txtEmpProject").val(data.Division);            
            $("#txtCurrentLocation").val(data.Location);
            $("#txtCurrentSupervisor").val(data.Supervisor);
            $("#hfLeaveUserId").val(data.Id);
        }
        else
        {
            $("#txtEmpName").val(data.FirstName);
            $("#txtDesignation").val(data.Designation);
            $("#txtEmpProject").val(data.Division);
            $("#hfCurrentSupervisorId").val(data.SupervisorId);
            $("#txtCurrentSupervisor").val(data.Supervisor);
            $("#txtCurrentLocation").val(data.Location);
            $("#hfTransferUser").val(data.Id);
            $("#hfCurrentDivision").val(data.CurrentDivisionId);
        }
        
    });
});

$(".divisions").on('change', function () {
    if($("#hfTransferUser").val() == 0)
    {
        errorMsg("Please select an Employee");
    }

    //alert($("#hfTransferUser").val());
});

$("#btnRequestTransfer").click(function () {
    $("#hfTransferTo").val($(".divisions").val());

    if ($("#txtEmpNo").val() == '')
    {
        errorMsg('Employee Number is Required');
    }
    else if ($("#txtEfectiveDate").val() == '')
    {
        errorMsg('Effective Date is Required');
    }
    else if ($("#txtSupervisorTo").val() == '') {
        errorMsg('Supervisor To Field is Required');
    }
    else if ($("#txtReason").val() == '') {
        errorMsg('Reason For Transfer is Required');
    }
    else
    {
        $("#frmRequestTransfer").submit();
    }
});

$(".btnaccepttranfer").click(function () {
    var id = this.id.replace("btnTransferAccept_", "");

    $("#hfTransferApprove").val(1);
    $("#hfTransferId").val(id);

    $("#frmApproveTransfer").submit();
});

$(".btnrejecttranfer").click(function () {
    var id = this.id.replace("btnTransferCancel_", "");

    $("#hfTransferApprove").val(0);
    $("#hfTransferId").val(id);

    $("#RejectTransferModal").modal("show");
});

$("#btnRejectTransfer").click(function () {
    $("#frmApproveTransfer").submit();
});