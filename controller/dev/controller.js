var airconsole = null;
var vm = null;
init();

function init() {
	airconsole = new AirConsole({"orientation": "portrait"});

	airconsole.onReady = function() {
    // Init the ViewManager
    vm = new AirConsoleViewManager(airconsole);
  };

  airconsole.onMessage = function(from, data) {
      // Show message on device screen
      if (from == AirConsole.SCREEN && data.action == "GAME_NOT_READY")
        vm.show("game-not-ready-view");
      if (from == AirConsole.SCREEN && data.action == "MAX_PLAYERS_REACHED")
        vm.show("max-players-reached-view");
      if (from == AirConsole.SCREEN && data.action == "CAN_JOIN")
        vm.show("submit-username-view");
      if (from == AirConsole.SCREEN && data.action == "YOU_ARE_HOST")
        vm.show("host-view");
      if (from == AirConsole.SCREEN && data.action == "WAIT_FOR_HOST")
        vm.show("waiting-for-host-view");
      if (from == AirConsole.SCREEN && data.action == "YOUR_CARDS") {
        vm.show("action-view");
        var src1 = "Cards/" + data.holeCard1 + ".svg";
        console.log(src1);
        var src2 = "Cards/" + data.holeCard2 + ".svg";
        showHoleCard('hole-card-1', src1, 50, 70, data.holeCard1);
        showHoleCard('hole-card-2', src2, 50, 70, data.holeCard2);
      }
      if (from == AirConsole.SCREEN && data.action == "YOUR_TURN") {
        vm.show("action-view");

        if (data.canFold)
          document.getElementById("fold-button").style.display = "block";
        else
          document.getElementById("fold-button").style.display = "none";

        if (data.canCheck)
          document.getElementById("check-button").style.display = "block";
        else
          document.getElementById("check-button").style.display = "none";

        if (data.canCall) {
          document.getElementById("call-button").style.display = "block";
          document.getElementById("call-button").innerHTML = "Call " + data.callAmount;
          document.getElementById("call-button").value = data.callAmount;
        }
        else
          document.getElementById("call-button").style.display = "none";

        if (data.canBet) {
          document.getElementById("bet-button").style.display = "block";
          document.getElementById("bet-button").innerHTML = "Bet " + data.minimumBetAmount;
          document.getElementById("bet-button").value = data.minimumBetAmount;
        }
        else
          document.getElementById("bet-button").style.display = "none";

        if (data.canRaise) {
          document.getElementById("raise-button").style.display = "block";
          document.getElementById("raise-button").innerHTML = "Raise to " + data.minimumRaiseSize;
          document.getElementById("raise-button").value = data.minimumRaiseSize;
        }
        else
          document.getElementById("raise-button").style.display = "none";
      }
      if (from == AirConsole.SCREEN && data.action == "WAIT_FOR_TURN") {
        document.getElementById("fold-button").style.display = "none";
        document.getElementById("check-button").style.display = "none";
        document.getElementById("call-button").style.display = "none";
        document.getElementById("bet-button").style.display = "none";
        document.getElementById("raise-button").style.display = "none";
      }
    };

  /*
   * Checks if this device is part of the active game.
   */
  /*airconsole.onActivePlayersChange = function(player) {
  var div = document.getElementById("player_id");
    if (player !== undefined) {
      vm.show('no-game-view');
    } else {
      vm.show('start-view');
    }
  };*/


  document.getElementById('submit-username').addEventListener("click", submitUsername);
  document.getElementById('start-game').addEventListener("click", startGame);

  document.getElementById('fold-button').addEventListener("click", fold);
  document.getElementById('check-button').addEventListener("click", check);
  document.getElementById('call-button').addEventListener("click", call);
  document.getElementById('bet-button').addEventListener("click", bet);
  document.getElementById('raise-button').addEventListener("click", raise);
 	
}

function fold() {
  var message = {
		'action': 'FOLD',
	};

	airconsole.message(airconsole.SCREEN, message);
}

function check() {
  var message = {
		'action': 'CHECK',
	};

	airconsole.message(airconsole.SCREEN, message);
}

function call() {
  var message = {
		'action': 'CALL',
    'callAmount': document.getElementById('call-button').value
	};

	airconsole.message(airconsole.SCREEN, message);
}

function bet() {
  var message = {
		'action': 'BET',
    'betAmount': document.getElementById('bet-button').value
	};

	airconsole.message(airconsole.SCREEN, message);
}

function raise() {
  var message = {
		'action': 'RAISE',
    'raiseAmount': document.getElementById('raise-button').value
	};

	airconsole.message(airconsole.SCREEN, message);
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

function startGame() {
	var message = {
		'action': 'SHUFFLE_UP_AND_DEAL'
	};

	airconsole.message(airconsole.SCREEN, message);
}

function showHoleCard(id, src, width, height, alt) {
  var img = document.createElement("img");
  img.src = src;
  img.width = width;
  img.height = height;
  img.alt = alt;
}