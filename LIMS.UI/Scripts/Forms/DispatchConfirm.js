(function () {
    var lims = window.lims = window.lims || {};
    if (lims.dispatchConfirm) {
        return;
    }

    var _context, _$container, _template;
    function _init() {
        if (!_context.list || !$.isArray(_context.list)) {
            return;
        }

        _$container = $("#dispatchConfirmModal");
        _template = $("#dispatchConfirmItemTemplate").html();

        var $body = $("tbody", _$container);
        _context.list.forEach(function (item) {
            var $tr = $(_template).appendTo($body);

            $tr.find("td:eq(0)").text(item.formNo);
            $tr.find("td:eq(1)").text(item.hospital);
            $tr.find("td:eq(2)").text(item.unit);
            $tr.find("td:eq(3)").text(item.product);
            $tr.find("td:eq(4)").text(item.dispatchCount);
            $tr.find("td:eq(5)").text(item.scanCount);

            $("td:eq(7)", $tr).find("input").datepicker();

            $tr.data("item", item);
        });

        _$container.on("click", "a.confirm", function () {
            var $tr = $(this).parents("tr"), item = $tr.data("item");

            if (!_validate($tr)) {
                return;
            }

            var barcode = $tr.find("input[class*='logistics-barcode']").val(),
                barcodeInfo = $tr.find("input[class*='logistics-info']").val(),
                batchNo = $("td:eq(6)", $tr).find("input").val(),
                expiredDate = $("td:eq(7)", $tr).find("input").datepicker("getDate");;

            $.ajax({
                url: window.baseUrl + "Main/Vendor/ConfirmDispatch",
                contentType: "application/json",
                type: "post",
                dataType: "json",
                data: JSON.stringify({
                    id: item.id,
                    batchNo: batchNo,
                    goodsExpiredDate: expiredDate,
                    logisticsBarcode: barcode,
                    logisticsInfo: barcodeInfo
                })
            }).done(function (response) {
                if (response.isSuccess) {
                    $tr.remove();
                }
            });
        });
    }

    function _validate($tr) {
        var isValid = true;

        var $error = $("#error").addClass("hidden");
        $(".has-error", $tr).removeClass("has-error");
        $("#error", _$container).addClass("hidden");

        $.each($("*[required]", $tr), function () {
            var $this = $(this);

            if ($this.val().trim() == "") {
                isValid = false;

                $this.parent().addClass("has-error");
            }
        });

        if (!isValid) {
            $error.removeClass("hidden");
        }

        return isValid;
    }

    lims.dispatchConfirm = {
        init: function (context) {
            _context = context;
            _init();
        }
    }
})();