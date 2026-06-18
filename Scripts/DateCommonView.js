$(function () {
    $.fn.datepicker.dates['th'] = {
        days: ["อาทิตย์", "จันทร์", "อังคาร", "พุธ", "พฤหัสบดี", "ศุกร์", "เสาร์"],
        daysShort: ["อา.", "จ.", "อ.", "พ.", "พฤ.", "ศ.", "ส."],
        daysMin: ["อา.", "จ.", "อ.", "พ.", "พฤ.", "ศ.", "ส."],
        months: ["มกราคม", "กุมภาพันธ์", "มีนาคม", "เมษายน", "พฤษภาคม", "มิถุนายน",
            "กรกฎาคม", "สิงหาคม", "กันยายน", "ตุลาคม", "พฤศจิกายน", "ธันวาคม"],
        monthsShort: ["ม.ค.", "ก.พ.", "มี.ค.", "เม.ย.", "พ.ค.", "มิ.ย.",
            "ก.ค.", "ส.ค.", "ก.ย.", "ต.ค.", "พ.ย.", "ธ.ค."],
        today: "Today",
        clear: "Clear",
        format: "mm/dd/yyyy",
        titleFormat: "MM yyyy", /* Leverages same syntax as 'format' */
        weekStart: 0
    };

    
    
    $("#SelectedDate").datepicker({
        changeMonth: true,
        changeYear: true,
        language: 'th',
        format: "dd/mm/yyyy",
        /*maxViewMode: 3,*/
        /*daysOfWeekHighlighted: "0,6",*/
        autoclose: true,
        showWeek: true,
        showOtherMonths: true,
        todayHighlight: true
        //onSelect: function (date) {
        //    $("#SelectedDate").addClass('active day');
        //}

    });

    $("#T_DATE").datepicker({

        language: 'en-en',
        format: "dd/mm/yyyy",
        maxViewMode: 3,
        daysOfWeekHighlighted: "0,6",
        autoclose: true,
        showWeek: true,
        showOtherMonths: true,
        todayHighlight: true,
        endDate: new Date(new Date().setDate(new Date().getDate() - 1)),
        setDate: new Date(new Date().setDate(new Date().getDate() - 1))
    });

    $("#B_DATE").datepicker({

        language: 'en-en',
        format: "dd/mm/yyyy",
        maxViewMode: 3,
        daysOfWeekHighlighted: "0,6",
        autoclose: true,
        showWeek: true,
        showOtherMonths: true,
        todayHighlight: true,
        //endDate: new Date(new Date().setDate(new Date().getDate() - 1)),
        //setDate: new Date(new Date().setDate(new Date().getDate() - 1))
    });

    $("#E_DATE").datepicker({

        language: 'en-en',
        format: "dd/mm/yyyy",
        maxViewMode: 3,
        daysOfWeekHighlighted: "0,6",
        autoclose: true,
        showWeek: true,
        showOtherMonths: true,
        todayHighlight: true,
        //endDate: new Date(new Date().setDate(new Date().getDate() - 1)),
        //setDate: new Date(new Date().setDate(new Date().getDate() - 1))
    });

    // Initialization for ES Users
    import { Input, initMDB } from "mdb-ui-kit";
    import { Dropdown, Collapse, initMDB } from "mdb-ui-kit";

    initMDB({ Dropdown, Collapse });
    initMDB({ Input });

    //Latitude Longitude
    function getLocation() {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(showPosition, showError);
        } else {
            alert("Geolocation is not supported by this browser.");
        }
    }

    function showPosition(position) {
        var latitude = position.coords.latitude;
        var longitude = position.coords.longitude;

        // Send data to MVC Controller
        fetch('/Home/SaveLocation', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ Latitude: latitude, Longitude: longitude })
        })
            .then(response => response.text())
            .then(data => {
                document.getElementById("location").innerText = "Location saved: " + latitude + ", " + longitude;
            })
            .catch(error => console.error('Error:', error));
    }

    function showError(error) {
        switch (error.code) {
            case error.PERMISSION_DENIED:
                alert("User denied the request for Geolocation.");
                break;
            case error.POSITION_UNAVAILABLE:
                alert("Location information is unavailable.");
                break;
            case error.TIMEOUT:
                alert("The request to get user location timed out.");
                break;
            case error.UNKNOWN_ERROR:
                alert("An unknown error occurred.");
                break;
        }
    }
});
