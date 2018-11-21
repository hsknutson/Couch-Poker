var airconsole = null;
var vm = null;

var src1;
var src2
var chipsCommitted = 0;
var canRaise = false;
var canBet = false;
var viewingCards = false;
var maximumRaiseSize = 0;
var maximumBetSize = 0;
var minimumRaiseSize = 0;
var minimumRaiseSize = 0;

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
        src1 = "svg-bundle.svg#" + data.holeCard1;
        console.log(src1);
        src2 = "svg-bundle.svg#" + data.holeCard2;
        showHoleCard('hole-card-1', src1, 50, 70, data.holeCard1);
        showHoleCard('hole-card-2', src2, 50, 70, data.holeCard2);
      }
      if (from == AirConsole.SCREEN && data.action == "UPDATE_CHIP_COUNT") {
        updateChipCount(data.chipCount);
      }
      if (from == AirConsole.SCREEN && data.action == "NOT_YOUR_TURN") {
        document.getElementById("waiting-statement").style.display = "block";
        document.getElementById("action-controls").style.display = "none";
      }
      if (from == AirConsole.SCREEN && data.action == "YOUR_TURN") {
        vm.show("action-view");
        document.getElementById("action-controls").style.display = "block";
        document.getElementById("waiting-statement").style.display = "none";

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
          canBet = true;
          document.getElementById("bet-button").style.display = "block";
          document.getElementById("bet-button").innerHTML = "Bet " + data.minimumBetAmount;
          document.getElementById("bet-button").value = data.minimumBetAmount;

          document.getElementById("minus-bet-button").value = data.betInterval;
          document.getElementById("plus-bet-button").value = data.betInterval;
          var halfPotBetSize = data.potSize/2;
          if (halfPotBetSize > data.maximumBetSize)
            halfPotBetSize = data.maximumBetSize;
          document.getElementById("half-pot-bet-button").value = halfPotBetSize;
          var potBetSize = data.potSize;
          if (potBetSize > data.maximumBetSize)
            potBetSize = data.maximumBetSize;
          document.getElementById("pot-bet-button").value = potBetSize;
          document.getElementById("max-bet-button").value = data.maximumBetSize;
          maximumBetSize = data.maximumBetSize;
          minimumBetSize = data.minimumBetAmount;

          document.getElementById("input-betting-amount").value = data.minimumBetAmount;

          var bettingSlider = document.getElementById("betting-amount-slider");
          bettingSlider.min = data.minimumBetAmount;
          bettingSlider.max = data.maximumBetSize;
          bettingSlider.value = data.minimumBetAmount;
        }
        else {
          canBet = false;
          document.getElementById("bet-button").style.display = "none";
        }
        if (data.canRaise) {
          canRaise = true;
          document.getElementById("raise-button").style.display = "block";
          document.getElementById("raise-button").innerHTML = "Raise to " + data.minimumRaiseSize;
          document.getElementById("raise-button").value = data.minimumRaiseSize;

          console.log("raise interval: " + data.raiseInterval +  " pot size: " + data.potSize);
          document.getElementById("minus-bet-button").value = data.raiseInterval;
          document.getElementById("plus-bet-button").value = data.raiseInterval;
          var halfPotSizeRaise = data.chipsCommitted + data.callAmount + 0.5 * (data.potSize + data.callAmount);
          if (halfPotSizeRaise > data.maximumRaiseSize)
            halfPotSizeRaise = data.maximumRaiseSize;
          document.getElementById("half-pot-bet-button").value = halfPotSizeRaise;
          var potSizeRaise = data.chipsCommitted + data.callAmount + (data.potSize + data.callAmount);
          if (potSizeRaise > data.maximumRaiseSize)
            potSizeRaise = data.maximumRaiseSize;
          document.getElementById("pot-bet-button").value = potSizeRaise;
          document.getElementById("max-bet-button").value = data.maximumRaiseSize;
          maximumRaiseSize = data.maximumRaiseSize;
          minimumRaiseSize = data.minimumRaiseSize;

          document.getElementById("input-betting-amount").value = data.minimumRaiseSize;

          var bettingSlider = document.getElementById("betting-amount-slider");
          bettingSlider.min = data.minimumRaiseSize;
          bettingSlider.max = data.maximumRaiseSize;
          bettingSlider.value = data.minimumRaiseSize;
        }
        else {
          canRaise = false;
          document.getElementById("raise-button").style.display = "none";
        }
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


  $("#player-hand-button").on("click", function(evt) {
    vm.show("action-view");
  });
  $("#hand-history-button").on("click", function(evt) {
    vm.show("hand-history-view");
  });
  $("#ranking-guide-button").on("click", function(evt) {
    vm.show("ranking-guide-view");
  });
  $("#about-button").on("click", function(evt) {
    vm.show("about-view");
  });
  $(".betting-button").on("click", function(evt) {
  });
  $("#betting-amount-slider").bind("input", function() {
    changeBetAmount($(this).val());
  });

  $('#input-betting-amount').bind("input", function() {
    changeBetAmountWithText($(this).val());
  });

  $(".betting-button").on("click", function(evt) {
    evt.stopPropagation();
    evt.preventDefault();
    evt.stopImmediatePropagation();
    var button;
    if (canBet)
      button = document.getElementById("bet-button");
    else if (canRaise)
      button = document.getElementById("raise-button");
    var $button = $(this);
    if ($button.text() == "-") {
      var newVal = button.value - $button.val();
      if (canBet && newVal < minimumBetAmount)
        newVal = minimumBetAmount;
      else if (canRaise && newVal < minimumRaiseSize)
        newVal = minimumRaiseSize;
      if (canBet)
        updateBetAmount(newVal);
      else if (canRaise)
        updateRaiseAmount(newVal);
    }
    else if ($button.text() == "+") {
      var newVal = Number(button.value) + Number($button.val());
      if (canBet && newVal > maximumBetSize)
        newVal = maximumBetSize;
      else if (canRaise && newVal > maximumRaiseSize)
        newVal = maximumRaiseSize;
      if (canBet)
        updateBetAmount(newVal);
      else if (canRaise)
        updateRaiseAmount(newVal);
    }
    else if ($button.text() == "1/2") {
      var newVal = $button.val();
      if (canBet)
        updateBetAmount(newVal);
      else if (canRaise)
        updateRaiseAmount(newVal);
    }
    else if ($button.text() == "POT") {
      var newVal = $button.val();
      if (canBet)
        updateBetAmount(newVal);
      else if (canRaise)
        updateRaiseAmount(newVal);
    }
    else if ($button.text() == "MAX") {
      var newVal = $button.val();
      if (canBet)
        updateBetAmount(newVal);
      else if (canRaise)
        updateRaiseAmount(newVal);
    }
  });
   
  $("card-row").on("click", function(evt) {
    if (viewingCards)
    {
      hideCards();
      viewingCards = !viewingCards;
    }
    else
    {
      revealCards(src1, src2);
      viewingCards = !viewingCards;
    }
  });
}

function revealCards(card1, card2) {
  document.getElementById("hole-card-1").style.background = url(card1);
  document.getElementById("hole-card-2").style.background = url(card2);
}

function hideCards() {
  document.getElementById("hole-card-1").style.background = url("As.svg");
  document.getElementById("hole-card-2").style.background = url("As.svg");
}

function updateBetAmount(amount) {
  document.getElementById("bet-button").value = amount;
  document.getElementById("bet-button").innerHTML = "Bet " + amount;
  var bettingSlider = document.getElementById("betting-amount-slider");
  bettingSlider.value = amount;
  document.getElementById('input-betting-amount').value = amount;
  
}

function changeBetAmountWithText(val) {
  if (val < 0)
    val = 0;
  if (canBet) {
    if (val > maximumBetSize)
      val = maximumBetSize;
    document.getElementById("bet-button").value = val;
    document.getElementById("bet-button").innerHTML = "Bet " + val;
  }
  else if (canRaise) {
    if (val > maximumRaiseSize)
      val = maximumRaiseSize;
    document.getElementById("raise-button").value = val;
    document.getElementById("raise-button").innerHTML = "Raise to " + val;
  }
  var bettingSlider = document.getElementById("betting-amount-slider");
  bettingSlider.value = val;
}

function changeBetAmount(val) {
  if (canBet) {
    document.getElementById("bet-button").value = val;
    document.getElementById("bet-button").innerHTML = "Bet " + val;
    document.getElementById('input-betting-amount').value = val;
  }
  else if (canRaise) {
    document.getElementById("raise-button").value = val;
    document.getElementById("raise-button").innerHTML = "Raise to " + val;
    document.getElementById('input-betting-amount').value = val;
  }
}

function updateRaiseAmount(amount) {
  document.getElementById("raise-button").value = amount;
  document.getElementById("raise-button").innerHTML = "Raise to " + amount;
  var bettingSlider = document.getElementById("betting-amount-slider");
  bettingSlider.value = amount;
  document.getElementById('input-betting-amount').value = amount;
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

function updateChipCount(chipCount) {
  document.getElementById('chip-count-text').innerHTML = chipCount + " chips";
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
  document.getElementById("username-text").innerHTML = username;
}

function startGame() {
	var message = {
		'action': 'SHUFFLE_UP_AND_DEAL'
	};

	airconsole.message(airconsole.SCREEN, message);
}

function showHoleCard(id, src, width, height, alt) {
  var img = document.getElementById(id);
  img.innerHTML = "<svg>" + "<use xlink:href=\"" + src + "\" /></svg>";
  img.width = width;
  img.height = height;
  img.alt = alt;
}

/***** IMPORTED JAVASCRIPT *****/

(function() {

  var hamburger = {
    navToggle: document.querySelector('.nav-toggle'),
    nav: document.querySelector('nav'),

    doToggle: function(e) {
      e.preventDefault();
      this.navToggle.classList.toggle('expanded');
      this.nav.classList.toggle('expanded');
    }
  };

  hamburger.navToggle.addEventListener('click', function(e) { hamburger.doToggle(e); });
  hamburger.nav.addEventListener('click', function(e) { hamburger.doToggle(e); });

}());