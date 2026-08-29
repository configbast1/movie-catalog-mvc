// Клієнтські адаптери для власних (кастомних) атрибутів валідації.
// Підключається після jquery.validate.unobtrusive і "навчає" його читати
// data-val-releaseyear / data-val-forbiddenwords / data-val-posterfile.
(function ($) {
    "use strict";

    if (!$ || !$.validator || !$.validator.unobtrusive) {
        return;
    }

    // ---------- ReleaseYearAttribute ----------
    $.validator.addMethod("releaseyear", function (value, element, params) {
        if (!value) {
            return true;
        }

        var year = parseInt(value, 10);
        if (isNaN(year)) {
            return false;
        }

        return year >= params.min && year <= params.max;
    });

    $.validator.unobtrusive.adapters.add("releaseyear", ["min", "max"], function (options) {
        options.rules["releaseyear"] = {
            min: parseInt(options.params.min, 10),
            max: parseInt(options.params.max, 10)
        };
        options.messages["releaseyear"] = options.message;
    });

    // ---------- ForbiddenWordsAttribute ----------
    $.validator.addMethod("forbiddenwords", function (value, element, params) {
        if (!value) {
            return true;
        }

        var text = value.toLowerCase();

        return !params.words.some(function (word) {
            return word.length > 0 && text.indexOf(word) !== -1;
        });
    });

    $.validator.unobtrusive.adapters.add("forbiddenwords", ["words"], function (options) {
        options.rules["forbiddenwords"] = {
            words: (options.params.words || "").toLowerCase().split("|")
        };
        options.messages["forbiddenwords"] = options.message;
    });

    // ---------- PosterFileAttribute ----------
    $.validator.addMethod("posterfile", function (value, element, params) {
        var files = element.files;
        if (!files || files.length === 0) {
            return true; // файл не обов'язковий
        }

        var file = files[0];
        var name = (file.name || "").toLowerCase();

        var extensionAllowed = params.extensions.some(function (extension) {
            return extension.length > 0 && name.slice(-extension.length) === extension;
        });

        if (!extensionAllowed) {
            return false;
        }

        return file.size <= params.maxbytes;
    });

    $.validator.unobtrusive.adapters.add("posterfile", ["extensions", "maxbytes"], function (options) {
        options.rules["posterfile"] = {
            extensions: (options.params.extensions || "").toLowerCase().split("|"),
            maxbytes: parseInt(options.params.maxbytes, 10)
        };
        options.messages["posterfile"] = options.message;
    });

    // ---------- анімація блоку з помилками при невдалій відправці ----------
    $(function () {
        $("form").on("invalid-form.validate", function () {
            var summary = $(this).find(".validation-summary");
            if (summary.length === 0) {
                return;
            }

            summary.removeClass("is-shaking");
            void summary[0].offsetWidth; // перезапуск CSS-анімації
            summary.addClass("is-shaking");
        });
    });
})(jQuery);
