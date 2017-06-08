(function () {
    var lims = window.lims = window.lims || {};
    if (lims.dispatchItemsView) {
        return;
    }

    var _context, _$container, _template, _form, _formItem, _barcodes, _printedBarcodes = {};
    function _init() {
        _form = null;
        _formItem = null;
        _barcodes = null;
        _printedBarcodes = {};

        if (!_context || !_context.id) {
            return;
        }

        _$container = $("#dispatchItemsViewModal");
        _template = $("#barcodeTemplate", _$container).html();

        $("#itemsList", _$container).on("click", "a.barcodes", function () {
            var $tr = $(this).parents("tr");
            var item = $tr.data("item");
            if (item) {
                _formItem = item;
                _loadBarcodes(item.serialId);
            }
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
            url: window.baseUrl + "Vendor/Dispatch/LoadItems",
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

        var $tbody = $("#itemsList tbody", _$container);
        $.each(_form.items, function (index, item) {
            var $tr = $("<tr><td/><td/><td/><td/><td/><td/><td><a class='btn btn-primary barcodes'>条码</a></td></tr>").appendTo($tbody);
            $tr.find("td:eq(0)").text(item.serialNo);
            $tr.find("td:eq(1)").text(item.batchNo);
            $tr.find("td:eq(2)").text(lims.util.formatDate(item.expiredDate));
            $tr.find("td:eq(3)").text(item.logisticsCode);
            $tr.find("td:eq(4)").text(item.logisticsContent);
            $tr.find("td:eq(5)").text(lims.util.formatDate(item.createdDate));

            $tr.data("item", item);
        });
    }

    function _loadBarcodes(serialId) {
        var data = { serialId: serialId };

        $.ajax({
            url: window.baseUrl + "GoodsSerial/GetBarcodesByRoot",
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
                    _showBarcodes(response.data.barcodes);
                }
            }
        });
    }

    function _showBarcodes(barcodes) {
        var $barcodes = $("#barcodes", _$container);
        $barcodes.empty();

        _barcodes = barcodes;

        if (!barcodes || barcodes.length == 0) {
            return;
        }


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
            barcodes: temp
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

    lims.dispatchItemsView = {
        init: function (context) {
            _context = context;

            _init();
        }
    }
})();