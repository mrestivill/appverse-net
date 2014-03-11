//Define a var item as a Class
var ClassItems = function () {
}

//Create the prototype functions and variables as private
ClassItems.prototype = function () {
    var idTable = "", idHeader = "", idFooter = "";
    var dbColumns = [], columns = [];
    var pageSize = 0, totalItems = 0;
    var controler = "";
    var url = "";

    //Function to Init variables in the Prototype Class
    //options: parameters
    function initClassParams(options) {
        idTable = options.idTable;
        idHeader = options.idHeader;
        idFooter = options.idFooter;
        dbColumns = options.dbColumns;
        columns = options.columns;
        pageSize = options.pageSize;
        totalItems = options.totalItems;
        controler = options.controler;
        url = options.url;
    }

    //Function to format the Table Header
    //criteria:fiel to be sorted
    //direction: true = ASC, otherwise DESC
    var formatHeader = function (critera, direction) {
        var header = $("#" + idHeader); //$("#trHeader");
        // Table Header Columns
        var rows = "";
        for (var key in columns) {
            var title = columns[key];
            var sorting = direction != null ? direction : true;
            if (critera != null && critera == dbColumns[key]) {
                title = direction ? title + "▲" : title += "▼";
                sorting = !sorting;
            }
            rows += "<th><a href='#' onclick=\"classItems.formatBody(1, '" + dbColumns[key] + "', " + sorting + "); return false;\">" + title + "</a></th>";
        }
        rows += "<th>&nbsp;</th>";
        header.html(rows);
    };

    //Function to format the Table Footer with Pager
    //pager: current page
    //criteria:fiel to be sorted
    //direction: true = ASC, otherwise DESC
    var formatFooter = function (pager, critera, direction) {
        var footer = $("#" + idFooter); //$("#trFooter");
        var tdFooter = "<td>";
        if (pager > 1) {
            if (critera == null)
                tdFooter += "<a href='#' onclick=\"classItems.formatBody(" + (pager - 1) + "); return false;\">Previous</a>&nbsp;&nbsp;";
            else
                tdFooter += "<a href='#' onclick=\"classItems.formatBody(" + (pager - 1) + ", '" + critera + "', " + direction + "); return false;\">Previous</a>&nbsp;&nbsp;";
        }
        else {
            tdFooter += "Previous&nbsp;&nbsp;";
        }
        if (pager * pageSize < totalItems) {
            if (critera == null)
                tdFooter += "<a href='#' onclick=\"classItems.formatBody(" + (pager + 1) + "); return false;\">Next</a>";
            else
                tdFooter += "<a href='#' onclick=\"classItems.formatBody(" + (pager + 1) + ", '" + critera + "', " + direction + "); return false;\">Next</a>";
        }
        else {
            tdFooter += "Next";
        }
        tdFooter += "</td>";
        footer.html(tdFooter);
    }

    //Function to format the Table Contain
    //pager: current page
    //criteria:fiel to be sorted
    //direction: true = ASC, otherwise DESC
    var formatBody = function (pager, critera, direction) {
        var tb = $("#" + idTable);
        var options = { page: pager };
        if (critera != null)
            options = { page: pager, criteria: critera, direction: direction != null ? direction : true };

        $.post(url, options, function (data) {
            tb.find("tr.tbRow").remove();
            pageSize = data.PageSize;
            totalItems = data.TotalItemsCount;
            formatHeader(critera, direction);

            var rows = "";
            var location;
            for (var i = 0, len = data.Items.length; i < len; i++) {
                if (data.Items[i].Location!=null) {
                    location = data.Items[i].Location.Name;
                }
                else { 
                    location = "";
                } 
                rows += "<tr class='tbRow'><td>" + data.Items[i].Id + "</td>" +
                        "<td>" + data.Items[i].Title + "</td>" +
                        "<td>" + data.Items[i].Description + "</td>" +
                        "<td>" + jDateToString(data.Items[i].Moment) + "</td>" +
                        "<td>" + location + "</td>" +
                        "<td>" + jNumberToString(data.Items[i].Value) + "</td>" +
                        "<td><a href='/" + controler + "/Edit/" + data.Items[i].Id + "'>Edit</a>&nbsp;|&nbsp;" +
                        "<a href='/" + controler + "/Details/" + data.Items[i].Id + "'>Details</a>&nbsp;|&nbsp;" +
                        "<a href='/" + controler + "/Delete/" + data.Items[i].Id + "'>Delete</a></td></tr>";
            }
            tb.append(rows);
            formatFooter(pager, critera, direction);
        });
    }

    //Function to format the DateTime value from Text to JSON object
    //dateValue: date value, universal date.
    function jDateToString(dateValue) {
        //use JSON2 or some JS library to parse the string
        var parseDate = dateValue.replace('/Date(', '').replace(')/', '');
        var jObject = JSON.parse('{"date":' + parseDate + '}');
        var dateValue = new Date(jObject.date);

        return Globalize.format(dateValue, 'd') + "&nbsp" +
               Globalize.format(dateValue, 'T');
    }

    //Function to format the numbers with globalize format
    //numValue: number to format
    function jNumberToString(numValue) {
        return Globalize.format(numValue, "N");
    }

    // Public attributes/functions
    return {
        initClassParams: initClassParams,
        formatBody: formatBody
    }
}();

//Creating an Instance Class
var classItems = new ClassItems();