$(document).on("click", 'a[href^="#"]', function (t) {
    t.preventDefault(), $("html, body").animate({
        scrollTop: $($.attr(this, "href")).offset().top + 3
    }, 500);
}), $("#slider").slick({
    slidesToShow: 1,
    slidesToScroll: 1,
    infinite: !0,
    autoplay: !0,
    dots: !1,
    speed: 1e3,
    autoplaySpeed: 5e3,
    arrows: !0,
    nextArrow: '<button type="button" class="slick-next arrow"><img src="/img/arrow-right.svg", alt="Arrow"></button>',
    prevArrow: '<button type="button" class="slick-prev arrow"><img src="/img/arrow-left.svg", alt="Arrow"></button>'
}), console.log("Hello");

var guid = '';

$(document).ready(function () {

    //Add new parcel
    $("#add-btn").on("click", function () {
        guid = '';
        showForm();
    });

    //Close form
    $(".modal__close").on("click", function () {
        $(".modal").addClass("is-hidden");
    });

    //Table row click
    $("#table tbody tr").click(function () {
        guid = $(this).find("td").eq(0).html();
        showForm();

        var selected = $(this).hasClass("yellow");
        $("#table tbody tr").removeClass("yellow");
        if (!selected)
            $(this).addClass("yellow");
    });

    //Save button
    $('#save-button').on("click", function () {
        saveParcel();
    });

    //Pay button
    $('.paid-btn').on("click", function () {
        let model = new Object();
        model.GUID = guid;
        model.Status = 2;

        $.ajax({
            type: "POST",
            url: "/Api/ChangeStatus",
            data: JSON.stringify(model),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response !== null) {
                    if (response.IsSuccess === true) {
                        Swal.fire({
                            title: 'Status changed!',
                            text: "Parcel has been paid!",
                            type: 'success',
                            showCancelButton: false,
                            confirmButtonColor: '#3085d6',
                            confirmButtonText: 'Ok!'
                        }).then((result) => {
                            if (result.value) {
                                location.reload();
                            }
                        });
                    }
                    else {
                        alert(response.Message);
                    }
                } else {
                    alert("Something went wrong");
                }
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    });
});

function showForm() {

    if (guid === '') {
        $('#header').text('Create a new parcel');
    }
    else {
        $('#header').text('Edit parcel');
    }

    $('#sender-name').val('');
    $('#sender-public-key').val('');
    $('#sender-address').val('');
    $('#amount').val('');
    $('#date-payment').val('');
    $('#account').val('');
    $('#comment').val('');

    $('#recipient-name').val('');
    $('#recipient-public-key').val('');
    $('#recipient-address').val('');
    $('#status').val('');
    $('#date-delivery').val('');
    $('#track').val('');

    $(".modal--form").removeClass("is-hidden");

    $.getJSON("/Api/Get?GUID=" + guid, function (res) {
        console.log(res);

        if (res.IsSuccess) {
            console.log(res.Parcel.GUID);
            $('#sender-name').val(res.Parcel.Sender.FullName);
            $('#sender-public-key').val(res.Parcel.Sender.PublicKey);
            $('#sender-address').val(res.Parcel.Sender.Address);
            $('#amount').val(res.Parcel.PaymentAmount);
            $('#date-payment').val(res.Parcel.PaymentDate);
            $('#account').val(res.Parcel.SafeAccount);
            $('#comment').val(res.Parcel.Comment);

            $('#recipient-name').val(res.Parcel.RecipientName);
            $('#recipient-public-key').val(res.Parcel.RecipientPublicKey);
            $('#recipient-address').val(res.Parcel.RecipientAddress);
            $('#status').val(convertStatus(res.Parcel.DeliveryStatus));
            $('#date-delivery').val(res.Parcel.ShipmentDate);
            $('#track').val(res.Parcel.TrackNumber);

            //Awaiting payment
            if (res.Parcel.DeliveryStatus === 1) {
                //do nothing
            } else {
                $('.paid-div-btn').hide();
            }
        }
        else {
            console.log('Error: ' + res.Message);
        }
    });
}

function saveParcel() {

    $("#parcelForm").validate({
        rules: {
            senderName: "required",
            senderPublicKey: "required",
            senderAddress: "required",
            amount: {
                required: true,
                number: true
            },
            accountNumber: "required",
            recipientName: "required",
            recipientPublicKey: "required",
            track: "required"
        },
        submitHandler: function (form) {

            let model = new Object();
            model.GUID = guid;
            model.Comment = $('#comment').val();

            model.Sender = new Object();
            model.Sender.PublicKey = $('#sender-public-key').val();
            model.Sender.Name = $('#sender-name').val();
            model.Sender.Address = $('#sender-address').val();

            model.Recipient = new Object();
            model.Recipient.PublicKey = $('#recipient-public-key').val();
            model.Recipient.Name = $('#recipient-name').val();
            model.Recipient.Address = $('#recipient-address').val();

            model.Payment = new Object();
            model.Payment.Amount = $('#amount').val();
            model.Payment.SafeAccount = $('#account').val();

            model.Delivery = new Object();
            model.Delivery.TrackNumber = $('#track').val();

            $.ajax({
                type: "POST",
                url: "/Api/Post",
                data: JSON.stringify(model),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response !== null) {
                        if (response.IsSuccess === true) {
                            let row = $("tr:contains(" + response.Parcel.GUID + ")");
                            if (row.length === 0) {
                                $('#table tbody tr:last').after('<tr><td style="display:none">' + response.Parcel.GUID + '</td><td>' + response.Parcel.RecipientName + '</td><td>' + response.Parcel.PaymentAmount + ' CS</td><td>' + jsonToDate(response.Parcel.CreatedAt, 'MM/DD/YYYY / h:MM') + '</td><td>' + convertStatus(response.Parcel.DeliveryStatus) + '</td></tr>');

                                //Table row click
                                $("#table tbody tr").click(function () {
                                    guid = $(this).find("td").eq(0).html();
                                    showForm();

                                    var selected = $(this).hasClass("yellow");
                                    $("#table tbody tr").removeClass("yellow");
                                    if (!selected)
                                        $(this).addClass("yellow");
                                });
                                location.reload(true);
                            }
                            else {
                                row.html('<td style="display:none">' + response.Parcel.GUID + '</td><td>' + response.Parcel.RecipientName + '</td><td>' + response.Parcel.PaymentAmount + ' CS</td><td>' + jsonToDate(response.Parcel.CreatedAt, 'MM/DD/YYYY / h:MM') + '</td><td>' + convertStatus(response.Parcel.DeliveryStatus) + '</td>');
                            }
                        }
                        else {
                            alert(response.Message);
                        }
                    } else {
                        alert("Something went wrong");
                    }

                    $(".modal").addClass("is-hidden");
                },
                failure: function (response) {
                    alert(response.responseText);
                },
                error: function (response) {
                    alert(response.responseText);
                }
            });
        }
    });


}

function convertStatus(status) {
    if (status === 1) return 'Awaiting payment';
    else if (status === 2) return 'Preparation for shipment';
    else if (status === 3) return 'Parcel sent';
    else if (status === 4) return 'Awaiting receipt';
    else if (status === 5) return 'Received';
}

function changeStatus(status) {

    let model = new Object();
    model.GUID = guid;
    model.Status = status;

    $.ajax({
        type: "POST",
        url: "/Api/ChangeStatus",
        data: JSON.stringify(model),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response !== null) {
                if (response.IsSuccess === true) {
                    Swal.fire({
                        title: 'Status changed!',
                        type: 'success',
                        showCancelButton: false,
                        confirmButtonColor: '#3085d6',
                        confirmButtonText: 'Ok!'
                    }).then((result) => {
                        if (result.value) {
                            location.reload();
                        }
                    });
                }
                else {
                    alert(response.Message);
                }
            } else {
                alert("Something went wrong");
            }
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
}

function deleteParcel() {

    let model = new Object();
    model.GUID = guid;

    $.ajax({
        type: "POST",
        url: "/Api/Delete",
        data: JSON.stringify(model),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response !== null) {
                if (response.IsSuccess === true) {
                    location.reload();
                    alert('Parcel deleted!');
                }
                else {
                    alert(response.Message);
                }
            } else {
                alert("Something went wrong");
            }
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
}

function jsonToDate(date, format) {
    return moment(date).format(format);
}
