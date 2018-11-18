var airconsole = null;
var vm = null;

function init() {
	airconsole = new AirConsole({"orientation": "portrait"});

	airconsole.onReady = function() {
    // Init the ViewManager
    vm = new AirConsoleViewManager(airconsole);
  };

  /*
   * Checks if this device is part of the active game.
   */
  airconsole.onActivePlayersChange = function(player) {
  var div = document.getElementById("player_id");
    if (player !== undefined) {
      vm.show('no-game-view');
    } else {
      vm.show('start-view');
    }
  };


  document.getElementById('submit-username')
  	.addEventListener("click", submitUsername);
}

function submitUsername() {
	username = document.getElementById('input-username').value;
	if (username == "") {
		alert("Please enter a username.");
	}

	var message = {
		'action': 'TAKE_SEAT',
		'username': username
	};

	airconsole.message(airconsole.SCREEN, message);
}