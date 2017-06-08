(function () {
    var lims = window.lims = window.lims || {};
    if (lims.dispatchMain) {
        return;
    }

    var _orderForms, _template, _$dispatchList;
    function _init() {
        _template = $("#barcodeTemplate").html();
        _$dispatchList = $("#dispatchList");

        var $formNo = $("#formNo");
        if (_orderForms) {
            _orderForms.forEach(function (item) {
                $("<option/>").appendTo($formNo).attr("value", item).text(item);
            });
        }

        lims.util.asNumberInput($formNo);
        $("#btnQuery").click(function () {
            _query($formNo.val(), $("#validFlag").is(":checked"));
        });

        $("#cancelValid").change(function () {
            if ($(this).is(":checked")) {
                $("#scanerState").text("扫描完成后，将取消入库").addClass("label-warning");
            }
            else {
                $("#scanerState").text("扫描完成后，点击右边”确认“，完成发货").removeClass("label-warning");
            }
        });

        $("#execScanner").change(function () {
            if ($(this).is(":checked")) {
                lims.barcodeScanner.exec($("#scannerInput"), function (barcode) {
                    var $item = _$dispatchList.find("div[barcode='" + barcode + "']");

                    if ($("#cancelValid").is(":checked")) {
                        _cancelValid($item, barcode);
                    }
                    else {
                        if ($item.length > 0 && !$item.hasClass("barcode-selecting") && !$item.hasClass("barcode-valid")) {
                            $item.addClass("barcode-selecting");

                            _valid($item, barcode);
                        }
                    }
                });
            }
            else 
                lims.barcodeScanner.clear();
        });

        $("#btnDispatch").click(function () {
            lims.window.pop("Main/Vendor/DispatchConfirm");
        });

        $("#dispatchList").on("click", "a.glyphicon-print", function () {
            var $this = $(this);
            var data = $this.parent().data("item");

            var info = {
                barcode: data.barcode,
                name: data.productName,
                vendor: data.vendorName
            };

            lims.util.print(info);
        });

        _query();
    }

    function _bindData(list) {
        _$dispatchList.empty();

        list.forEach(function (item) {
            _appendItem(item);
        });
    }

    function _appendItem(item) {
        var $item = $(_template).appendTo(_$dispatchList);

        $item.find("a").removeClass("hidden");

        $item.attr("barcode", item.barcode);
        $item.data("item", item);
        $item.find("p:eq(1)").text(item.productName);
        $item.find("p:eq(2)").text(item.barcode);

        if (item.isValid) {
            $item.addClass("barcode-valid");
        }
    }

    function _query(formNo, isValid) {
        var data = {
            formNo: formNo,
            isValid: isValid || false
        };

        $.ajax({
            url: window.baseUrl + "Main/Vendor/QueryDispatch",
            contentType: "application/json",
            type: "post",
            dataType: "json",
            data: JSON.stringify(data)
        }).done(function (response) {
            if (response.isSuccess) {
                _bindData(response.data);
            }
        });
    }

    function _valid($item, barcode) {
        if (barcode == "") {
            return;
        }

        $.ajax({
            url: window.baseUrl + "Main/Vendor/ValidDispatch",
            contentType: "application/json",
            type: "post",
            dataType: "json",
            data: JSON.stringify({ barcode: barcode })
        }).done(function (response) {
            if (response.isSuccess) {
                $item.addClass("barcode-valid");
            }
        });
    }

    function _cancelValid($item, barcode) {
        if (barcode == "") {
            return;
        }

        $.ajax({
            url: window.baseUrl + "Main/Vendor/cancelValidDispatch",
            contentType: "application/json",
            type: "post",
            dataType: "json",
            data: JSON.stringify({ barcode: barcode })
        }).done(function (response) {
            if (response.isSuccess) {
                if ($item.length == 0) {
                    _appendItem(response.data);
                }
                else {
                    $item.removeClass("barcode-valid barcode-selecting");
                }
            }
        });
    }

    lims.dispatchMain = {
        init: function (orderForms) {
            _orderForms = orderForms;

            _init();
        }
    }
})();