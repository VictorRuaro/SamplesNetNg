$.validator.unobtrusive.adapters.add('existencias', ['url', 'parametername', 'parametervalue', 'additionalfields'], function (options) {
    options.rules['existencias'] = options.params;
    options.messages['existencias'] = options.message;
});

$.validator.addMethod('existencias', function (value, element, parameters) {

    var isValid = true;

    var url = parameters.url;
    var parameterName = parameters.parametername;
    var oldvalue = parameters.parametervalue;

    //var objectmodel = parameters.object;
    var additionalfields = parameters.additionalfields;

    oldvalue = (oldvalue == null ? '' : oldvalue).toString();

    var actualvalue = {};

    actualvalue = $(element).val();
    
    

        var data = {};
        var valida = false;

        if(additionalfields!=null) 
        {
            var obj = additionalfields;
            data = jQuery.parseJSON(obj) ;
        }

        data[parameterName] = value;

        //if (objectmodel != null) {
        //    var obj = objectmodel;
        //    data = jQuery.parseJSON(obj) ;
        //}
        
        $.ajax({
            url: url,
            data: data,
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                valida = response === true;
            }
            ,error: function (jqXHR, textStatus, errorThrown) {
                    //debugger
                }
        });

        isValid = valida;

    

    return isValid;
});