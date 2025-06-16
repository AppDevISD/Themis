(function () {
	const InactivityTimer = {
		idleMinutes: 0,
		timeoutMinutes: 10,
		intervalId: null,
		debug: false,

		log: function (message) {
			if (this.debug) {
				console.log(message);
			}
		},

		warn: function (message) {
			if (this.debug) {
				console.warn(message);
			}
		},

		init: function (timeoutInMinutes) {
			this.timeoutMinutes = timeoutInMinutes || this.timeoutMinutes;
			this.resetTimer(); // make sure we're at 0
			this.bindEvents();
			this.startTimer();
			this.log(`InactivityTimer initialized with timeout: ${this.timeoutMinutes} minutes`);
		},

		bindEvents: function () {
			const reset = this.resetTimer.bind(this);
			['keypress', 'scroll', 'click'].forEach(event =>
				document.addEventListener(event, reset)
			);
		},

		startTimer: function () {
			this.intervalId = setInterval(() => {
				this.idleMinutes++;
				this.log(`InactivityTimer: ${this.idleMinutes} minutes idle`);
				if (this.idleMinutes >= this.timeoutMinutes) {
					this.triggerPostback();
					this.resetTimer(); // reset after triggering
				}
			}, 60001); // every 1 minute
		},

		resetTimer: function () {
			this.idleMinutes = 0;
		},

		triggerPostback: function () {
			const triggerButton = document.querySelector("[data-inactivity-trigger='true']");
			if (triggerButton) {
				this.log(`InactivityTimer: Triggering refresh`);
				triggerButton.click();
			} else {
				this.warn('InactivityTimer: No valid trigger found.');
			}
		}
	};

	window.InactivityTimer = InactivityTimer;

	window.addEventListener('load', function () {
		InactivityTimer.init(); // Default to 10 min
	});
})();