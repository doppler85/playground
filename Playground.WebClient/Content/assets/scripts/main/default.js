$(document).ready(function () {

	$('.user-logged-in li .show-notifications').click(function () {
		if ($(this).hasClass('active')) {
			$(this).removeClass('active');
			$(this).next('.notifications-list').hide();
		} else {
			$(this).addClass('active');
			$(this).next('.notifications-list').show();
		}
	});

});