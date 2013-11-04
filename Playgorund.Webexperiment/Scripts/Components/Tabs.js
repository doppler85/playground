$(document).ready(function () {
    $('.tab-content').hide();
    $('ul.tabs li:first').addClass('selected').show();
    $('.tab-content:first').show();

    //On Click 
    $('ul.tabs li').click(function () {
        $('ul.tabs li').removeClass('selected');
        $(this).addClass('selected');
        $('.tab-content').hide();
        var activeTab = $(this).find('a').attr('href');
        $(activeTab).show();

        // change image on homepage
        var newImage = $(this).index(),
			thumbLenght = $('.thumbnails img').length;
        if (thumbLenght) {
            $('.thumbnails img').fadeOut(400).removeClass('active').eq(newImage).fadeIn(400).addClass('active');
        }
        return false;
    });
});