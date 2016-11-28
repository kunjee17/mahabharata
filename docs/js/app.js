$(function () {
    console.log('Ath shree Mahabharata katha');
    terms.map(function (term) {
        // console.log(term.Bookno);
        $('.nav.nav-tabs').append('<li><a href="#' + term.Bookno + '" data-toggle="pill">' + term.Bookno + '</a></li>');
        $('.tab-content').append('<div class="tab-pane fade" id="' + term.Bookno + '"><div id="graph' + term.Bookno + '"></div><p>' + term.Bookno + '</p></div>');
    });

});