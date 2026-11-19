$.validator.unobtrusive.adapters.add('requiredif', ['property', 'desiredvalue', 'isinverted', 'validatewhennullorempty'], function (options) {
    options.rules['requiredif'] = options.params;
    options.messages['requiredif'] = options.message;
});

$.validator.addMethod('requiredif', function (value, element, parameters) {
    
    var desiredvalue = parameters.desiredvalue;
    var isinverted = parameters.isinverted;
    var validatewhennullorempty = parameters.validatewhennullorempty

    desiredvalue = (desiredvalue == null ? '' : desiredvalue).toString();
    var controlType = $("input[id$='" + parameters.property + "']").attr("type");
    var actualvalue = {};

    if (controlType == "checkbox" || controlType == "radio") {
        var control = $("input[id$='" + parameters.property + "']:checked");
        actualvalue = control.val();

        if (actualvalue == undefined) {
            var valor = $("input[id$='" + parameters.property + "']").prop('checked');
            if (valor !== undefined) {
                actualvalue = valor;
            }
        }

    } else {
        if (controlType == undefined) {
            var control = $("#" + parameters.property);
            actualvalue = $(control).val();
        }
        else {
            actualvalue = $("input[id$='" + parameters.property + "']").val();
        }
    }

    if (validatewhennullorempty == 'true') {

        if (isinverted == 'true') {
            if ($.trim(desiredvalue).toLowerCase() !== $.trim(actualvalue).toLocaleLowerCase()) {
                var isValid = false;
                if ($.isNumeric(desiredvalue) && value === '0')
                { 
                    isValid = $.validator.methods.required.call(this, '', element, parameters);
                } else {
                    isValid = $.validator.methods.required.call(this, value, element, parameters);
                }
                return isValid;
            }
        } else {
            if ($.trim(desiredvalue).toLowerCase() === $.trim(actualvalue).toLocaleLowerCase()) {
                var isValid = $.validator.methods.required.call(this, value, element, parameters);
                return isValid;
            }
        }
    }

    return true;
});