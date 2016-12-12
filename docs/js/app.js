$(document).ready(function () {
    console.log('Ath shree Mahabharata katha');

    var drawColumnChart = function (chartid, bookName, bookdata) {
        Highcharts.chart(chartid, {
            chart: {
                type: 'column'
            },
            title: {
                text: bookName + ' Analysis'
            },
            tooltip: {
                headerFormat: '<span style="font-size:11px">{series.name}</span><br>',
                pointFormat: '<span style="color:{point.color}">{point.name}</span>: <b>{point.y}</b> of total<br/>'
            },
            xAxis: {
                type: 'category'
            },
            yAxis: {
                title: {
                    text: 'Total word frequency'
                }

            },
            plotOptions: {
                series: {
                    borderWidth: 0,
                    dataLabels: {
                        enabled: true,
                        format: '{point.y:.3f}%'
                    }
                }
            },
            series: [{
                name: bookName,
                colorByPoint: true,
                data: bookdata
                // data: [{
                //     name: 'Microsoft Internet Explorer',
                //     y: 56.33
                // }, {
                //     name: 'Anticipation',
                //     y: 24.03,
                // }, {
                //     name: 'Firefox',
                //     y: 10.38
                // }, {
                //     name: 'Safari',
                //     y: 4.77
                // }, {
                //     name: 'Opera',
                //     y: 0.91
                // }, {
                //     name: 'Proprietary or Undetectable',
                //     y: 0.2
                // }]
            }]
        });
    };


    var generateIdName = function (name) {
        return name.replace(' ', '-');
    };

    var activaTab = function (tab) {
        $('.nav-tabs a[href="#' + tab + '"]').tab('show');
    };

    $.getJSON('js/emotional.json')
        .done(function (books) {
            console.log(books);
            books.map(function (book) {
                $('.nav.nav-tabs').append('<li><a href="#' + generateIdName(book.Word) + '" data-toggle="pill">' + book.Word + '</a></li>');
                $('.tab-content')
                    .append('<div class="tab-pane fade" id="' + generateIdName(book.Word) + '"><strong>' + book.Word + '</strong><br/><br/><br/><div id="' + generateIdName(book.Word) + '-data"></div></div>');
                $('#' + generateIdName(book.Word) + '-data').append('<div id="' + generateIdName(book.Word) + '-columnchart" style="min-width:310 px; height: 400px; max-width: 600px; margin 0 auto"></div>');
                var bookdata = _.chain(book)
                    .map(function (y, name) {
                        return { y: y, name: name };
                    })
                    .filter(function (o) {
                        return o.name !== 'Word';
                    })
                    .value();

                drawColumnChart(generateIdName(book.Word + '-columnchart'), book.Word, bookdata);
            });
            activaTab(generateIdName(books[0].Word));
        })
        .fail(function (err) {
            console.log(err);
        })
        ;

});

$(function () {


    // var ctx = document.getElementById('chart-sample');
    // console.log(ctx);


    // terms.map(function (term) {
    //     // console.log(term.Bookno);
    //     $('.nav.nav-tabs').append('<li><a href="#' + term.Bookno + '" data-toggle="pill">' + term.Bookno + '</a></li>');
    //     $('.tab-content').append('<div class="tab-pane fade" id="' + term.Bookno + '"><div id="graph' + term.Bookno + '"></div><p>' + term.Bookno + '</p></div>');
    // });

});