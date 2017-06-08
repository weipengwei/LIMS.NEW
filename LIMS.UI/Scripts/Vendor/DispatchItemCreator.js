(function () {
    var lims = window.lims = window.lims || {};
    if (lims.dispatchItemCreator) {
        return;
    }

    var _context, _refresh, _$container, _template, _form, _formItem, _barcodes, _printedBarcodes = {};
    function _init() {
        _form = null;
        _formItem = null;
        _barcodes = null;
        _printedBarcodes = {}

        if (!_context || !_context.id) {
            return;
        }

        _$container = $("#dispatchItemCreatorModal");
        _template = $("#barcodeTemplate", _$container).html();

        $("#expiredDate", _$container).datepicker();

        $("#btnConfirm", _$container).click(function () {
            _confirm();
        });

        $("#barcodes", _$container).on("click", "a.glyphicon-print", function () {
            _printBarcodes([$(this).data("barcode")]);
        });

        $("#btnPrint", _$container).click(function () {
            if (!_barcodes || _barcodes.length == 0) {
                return;
            }

            unprinted = $("#printType", _$container).is(':checked');
            printingBarcodes = _barcodes.concat([]);

            _batchPrint("", unprinted, printingBarcodes);

            var intervalId = window.setInterval(function () {
                _batchPrint(intervalId, unprinted, printingBarcodes);
            }, 1000);
        });

        _load();
    }

    function _load() {
        var data = {
            id: _context.id
        };

        $.ajax({
            url: window.baseUrl + "Vendor/Dispatch/Load",
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

        _form = data;

        $("#product", _$container).text(data.product);
        $("#hospital", _$container).text(data.hospital);
        $("#total", _$container).text(data.total);
        $("#dispatchedCount", _$container).text(data.dispatchedCount);

        var max = data.total - data.dispatchedCount;
        var min = max == 0 ? 0 : 1;

        lims.util.numericBinder($("#count", _$container), { point: 0, min: min, max: max });
    }

    function _confirm() {
        $("#error", _$container).addClass("hidden");
        if (!lims.dataHelper.validate(_$container)) {
            $("#error", _$container).text("请输入必填项！").removeClass("hidden");
            return;
        }

        var data = {
            dispatchId: _context.id,
            batchNo: $("#batchNo", _$container).val(),
            expiredDate: $("#expiredDate", _$container).datepicker("getDate"),
            count: $("#count", _$container).val()
        }

        if (data.count <= 0) {
            $("#error", _$container).text("发放数量必须大于零！").removeClass("hidden");
            return;
        }

        _formItem = data;

        $.ajax({
            url: window.baseUrl + "Vendor/Dispatch/CreateItem",
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
                    _formItem.serialId = response.data.serialId;
                    _formItem.serialNo = response.data.serialNo;

                    _showPrintUI();
                    _showBarcodes(response.data.barcodes);

                    _refresh && _refresh();
                }
            }
        });
    }

    function _showPrintUI() {
        $("#printType", _$container).parent().removeClass("hidden");
        $("#btnPrint", _$container).removeClass("hidden");
        $("#btnConfirm", _$container).addClass("hidden");

        $('<p class="form-control-static"></p>').appendTo($("#batchNo", _$container).parent().empty()).text(_formItem.batchNo);
        $('<p class="form-control-static"></p>').appendTo($("#expiredDate", _$container).parent().empty()).text(lims.util.formatDate(_formItem.expiredDate));
        $('<p class="form-control-static"></p>').appendTo($("#count", _$container).parents("div.col-sm-3").empty()).text(_formItem.count);
    }

    function _showBarcodes(barcodes) {
        _barcodes = barcodes;

        if (!barcodes || barcodes.length == 0) {
            return;
        }

        var $barcodes = $("#barcodes", _$container);

        $.each(barcodes, function () {
            var $barcode = $(_template).appendTo($barcodes);

            $barcode.attr("barcode", "--" + this.barcode + "--");
            $barcode.find("p").text(this.barcode);
            $barcode.find("a").data("barcode", this.barcode);

            if (this.isPrinted) {
                _printedBarcodes[this.barcode] = true;

                $barcode.addClass("barcode-printed");
            }
        });
    }

    function _batchPrint(intervalId, unprinted, printingBarcodes) {
        if (!printingBarcodes || printingBarcodes.length == 0) {
            if (intervalId) {
                window.clearInterval(intervalId);
            }
            return;
        }

        var barcodes = [];
        var shiftItem;
        while (barcodes.length < 5 && !!(shiftItem = printingBarcodes.shift())) {
            if (unprinted) {
                if (!_printedBarcodes[shiftItem.barcode]) {
                    barcodes.push(shiftItem.barcode);
                }
            }
            else {
                barcodes.push(shiftItem.barcode);
            }
        }

        if (barcodes.length == 0) {
            if (intervalId) {
                window.clearInterval(intervalId);
            }
            return;
        }

        _printBarcodes(barcodes);
    }

    function _printBarcodes(barcodes) {
        $.each(barcodes, function (i, barcode) {
            lims.util.print({
                serialNo: _formItem.serialNo,
                barcode: barcode,
                name: _form.product,
                vendor: _form.vendor,
                batchNo: _formItem.batchNo,
                expiredDate: lims.util.formatDate(_formItem.expiredDate)
            });
        });

        _updatePrintStatus(_formItem.serialId, barcodes);
    }

    function _updatePrintStatus(serialId, barcodes) {
        if (!barcodes || barcodes.length == 0) {
            return;
        }

        var temp = [];
        $.each(barcodes, function (i, barcode) {
            if (!_printedBarcodes[barcode]) {
                temp.push(barcode);
            }
        });

        if (temp.length == 0) {
            return;
        }

        var data = {
            serialId: serialId,
            barcodes: barcodes
        };

        $.ajax({
            url: window.baseUrl + "GoodsSerial/UpdatePrint",
            contentType: "application/json",
            type: "post",
            dataType: "json",
            data: JSON.stringify(data)
        }).done(function (response) {
            if (response.isSuccess) {
                _showPrinted(barcodes);
            }
        });
    }

    function _showPrinted(barcodes) {
        var $barcodes = $("#barcodes ", _$container);;
        $.each(barcodes, function (i, barcode) {
            _printedBarcodes[barcode] = true;
            $("div.barcode[barcode='" + _formatAttrValue(barcode) + "']", $barcodes).addClass("barcode-printed");
        });
    }

    function _formatAttrValue(input) {
        return "--" + input + "--";
    }

    lims.dispatchItemCreator = {
        init: function (context) {
            _context = context;

            _init();
        },
        refresh: function (callback) {
            _refresh = callback
        }
    }
})();