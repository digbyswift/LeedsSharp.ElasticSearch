ls.loader = (function (window, document, $, undefined) {

	"use strict";

	var pub = {};

	pub.init = function () {

		$(function () {

			$('[data-loader]').each(function (i, obj) {
				load($(obj));
			});

		});

	};

	pub.reload = function (selector) {
		load($(selector));
	};

	function load($target) {

		console.log('loading');

		$.ajax({
			type: 'get',
			url: $target.data('loader-url'),
			data: $target.data('query-data'),
			success: function (data) {
				$target.html(data);
			}
		});

	}

	return pub;

})(window, document, jQuery);

