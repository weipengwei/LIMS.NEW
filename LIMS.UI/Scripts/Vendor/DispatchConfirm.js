(function () {
    var lims = window.lims = window.lims || {};
    if (lims.dispatchConfirm) {
        return;
    }

    var _$container, _refresh, _goodsSerial;
    function _init() {
        _$container = $("#dispatchConfirmModal");

        $("#expiredDate", _$container).datepicker();

        $("#btnRescan", _$container).click(function () {
            _initEvn();
            _startScan();
        });

        $("#btnClose", _$container).click(function () {
            _refresh && _refresh();
        });

        $("#btnConfirm", _$container).click(function () {
            _confirm();
        });

        _startScan();
    }

    function _startScan() {
        lims.barcodeScanner.exec($("#scannerInput"), function (barcode) {
            _initEvn();
            _load(barcode);
        });
    }

    function _load(barcode) {
        var data = {
            barcode: barcode,
            formKind: "dispatch_item"
        };

        $.ajax({
            url: window.baseUrl + "GoodsSerial/LoadByBarcode",
            contentType: "application/json",
            type: "post",
            dataType: "json",
            data: JSON.stringify(data)
        }).done(function (response) {
            if (response.isSuccess) {
                if (response.message) {
                    $("#error", _$container).text(response.message).removeClass("hidden");
                }
                else {
                    _bind(response.data);
                }
            }
        });
    }

    function _bind(data) {
        if (!data) {
            return;
        }

        _goodsSerial = data;

        $("#serialNo", _$container).text(data.serialNo);
        $("#product", _$container).text(data.product);
        $("#vendor", _$container).text(data.vendor);
        $("#count", _$container).text(data.count);
        $("#batchNo", _$container).text(data.batchNo);
        $("#expiredDate", _$container).text(lims.util.formatDate(data.expiredDate));
        $("#logisticsCode", _$container).val(data.logisticsCode);
        $("#logisticsContent", _$container).val(data.logisticsContent);

        $("#goodsSerial", _$container).removeClass("hidden");
        $("#scannerInput", _$container).addClass("hidden");
        $("#btnRescan", _$container).removeClass("hidden");
        $("#btnConfirm", _$container).removeClass("hidden");
        $("#logisticsCode", _$container).focus();
        lims.barcodeScanner.clear();
    }

    function _initEvn() {
        _goodsSerial = null;
        
        $("#batchNo", _$container).val("");
        $("#expiredDate", _$container).val("");
        $("#logisticsCode", _$container).val("");
        $("#logisticsContent", _$container).val("");

        $("#error", _$container).addClass("hidden");
        $("#scannerInput", _$container).removeClass("hidden");
        $("#btnRescan", _$container).addClass("hidden");
        $("#btnConfirm", _$container).addClass("hidden");
        $("#goodsSerial", _$container).addClass("hidden");
    }

    function _confirm() {
        $("#error", _$container).addClass("hidden");
        if (!_goodsSerial || !_goodsSerial.serialId) {
            return;
        }

        if (!lims.dataHelper.validate($("#goodsSerial", _$container))) {
            $("#error", _$container).text("请输入必填项！").removeClass("hidden");
            return;
        }

        var data = {
            serialId: _goodsSerial.serialId,
            logisticsCode: $("#logisticsCode", _$container).val(),
            logisticsContent: $("#logisticsContent", _$container).val()
        }

        $.ajax({
            url: window.baseUrl + "Vendor/Dispatch/Pass",
            contentType: "application/json",
            type: "post",
            dataType: "json",
            data: JSON.stringify(data)
        }).done(function (response) {
            if (response.isSuccess) {
                if (response.message) {
                    $("#error", _$container).text(response.message).removeClass("hidden");
                }
                else {
                    _initEvn();
                    _startScan();
                }
            }
        });
    }

    lims.dispatchConfirm = {
        init: function () {
            _init();
        },
        refresh: function (callback) {
            _refresh = callback
        }
    }
})();