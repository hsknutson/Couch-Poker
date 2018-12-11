var airconsole = null;
var vm = null;
init();

var src1;
var src2
var wager = 0;
var canRaise = false;
var canBet = false;
var viewingCards = false;
var maximumRaiseAmount = 0;
var maximumBetAmount = 0;
var minimumRaiseAmount = 0;
var minimumRaiseAmount = 0;
var showedShakeAnimation = false;
var shakeDisabled = false;

function init() {
	airconsole = new AirConsole({"orientation": "portrait"});

	airconsole.onReady = function() {
    // Init the ViewManager
    vm = new AirConsoleViewManager(airconsole);
  };

  document.getElementById("nav-toggle").style.display = "none";
  document.getElementById("notification-badge").style.display = "none";
  document.getElementById("notification-badge-two").style.display = "none";
  document.getElementById("action-controls").style.display = "none";
  document.getElementById('hand-history-expanded').style.display = "none";

  airconsole.onMessage = function(from, data) {
      // Show message on device screen
      if (from == AirConsole.SCREEN && data.action == "GAME_NOT_READY")
        vm.show("game-not-ready-view");
      if (from == AirConsole.SCREEN && data.action == "ROUND_END") {
        document.getElementById("waiting-info").style.display = "block";
        document.getElementById("waiting-info").innerHTML = "Waiting for the next hand to be dealt...";
        document.getElementById('hole-card-1').style.backgroundImage = 'url(Cards/card_placeholder.png)';
        document.getElementById('hole-card-2').style.backgroundImage = 'url(Cards/card_placeholder.png)';
      }
      if (from == AirConsole.SCREEN && data.action == "MAX_PLAYERS_REACHED")
        vm.show("max-players-reached-view");
      if (from == AirConsole.SCREEN && data.action == "CAN_JOIN")
        vm.show("submit-username-view");
      if (from == AirConsole.SCREEN && data.action == "YOU_ARE_HOST")
        vm.show("host-view");
      if (from == AirConsole.SCREEN && data.action == "WAIT_FOR_HOST")
        vm.show("waiting-for-host-view");
      if (from == AirConsole.SCREEN && data.action == "KNOCKED_OUT")
        vm.show("knocked-out");
      if (from == AirConsole.SCREEN && data.action == "YOUR_CARDS") {
        src1 = "Cards/" + data.holeCard1 + ".png";
        src2 = "Cards/" + data.holeCard2 + ".png";
        cardsShowing = false;
        document.getElementById('hole-card-1').style.backgroundImage = 'url(Cards/card_back.png)';
        document.getElementById('hole-card-2').style.backgroundImage = 'url(Cards/card_back.png)';
        document.getElementById("waiting-info").innerHTML = "";
        canToggleCards = true;
        if (!showedShakeAnimation) {
          document.getElementById('hole-card-1').className += "shake";
          document.getElementById('hole-card-2').className += "shake";
          showedShakeAnimation = true;
        }
      }
      if (from == AirConsole.SCREEN && data.action == "MUCK_CARDS") {
        document.getElementById('hole-card-1').style.backgroundImage = "";
        document.getElementById('hole-card-2').style.backgroundImage = "";
        canToggleCards = false;
      }
      if (from == AirConsole.SCREEN && data.action == "HIDE_ACTIVE_PLAYER") {
        document.getElementById('waiting-info').style.display = "none";
      }
      if (from == AirConsole.SCREEN && data.action == "GAME_HAS_STARTED") {
        document.getElementById("nav-toggle").style.display = "block";
        document.getElementById("waiting-info").style.display = "block";
        document.getElementById("waiting-info").innerHTML = "Waiting for the first hand to be dealt...";
        document.getElementById('hole-card-1').style.backgroundImage = 'url(Cards/card_placeholder.png)';
        document.getElementById('hole-card-2').style.backgroundImage = 'url(Cards/card_placeholder.png)';
        vm.show("action-view");
      }
      if (from == AirConsole.SCREEN && data.action == "UPDATE_CHIP_COUNT") {
        updateChipCount(data.chipCount);
      }
      if (from == AirConsole.SCREEN && data.action == "HIDE_ACTION_CONTROLS") {
        document.getElementById("action-controls").style.display = "none";
        document.getElementById("notification-badge").style.display = "none";
        document.getElementById("notification-badge-two").style.display = "none";
      }
      if (from == AirConsole.SCREEN && data.action == "NOT_YOUR_TURN") {
        document.getElementById("notification-badge").style.display = "none";
        document.getElementById("notification-badge-two").style.display = "none";
        document.getElementById("action-controls").style.display = "none";
        document.getElementById("waiting-info").style.display = "block";
        document.getElementById("waiting-info").innerHTML = "Waiting for " + data.active_player_name + " to act...";
      }
      if (from == AirConsole.SCREEN && data.action == "YOUR_TURN") {
        console.log("pot size: " + data.potSize);
        document.getElementById("waiting-info").style.display = "none";
        document.getElementById("action-controls").style.display = "block";

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
          document.getElementById("betting-controls").style.display = "block";
          canBet = true;
          document.getElementById("bet-button").style.display = "block";
          document.getElementById("bet-button").innerHTML = "Bet " + data.minimumBetAmount;
          document.getElementById("bet-button").value = data.minimumBetAmount;

          document.getElementById("minus-bet-button").value = data.betInterval;
          document.getElementById("plus-bet-button").value = data.betInterval;
          var halfPotBetSize = data.potSize/2;
          if (halfPotBetSize > data.maximumBetAmount)
            halfPotBetSize = data.maximumBetAmount;
          document.getElementById("half-pot-bet-button").value = Math.round(halfPotBetSize);
          var potBetSize = data.potSize;
          if (potBetSize > data.maximumBetAmount)
            potBetSize = data.maximumBetAmount;
          document.getElementById("pot-bet-button").value = potBetSize;
          document.getElementById("max-bet-button").value = data.maximumBetAmount;
          maximumBetAmount = data.maximumBetAmount;
          minimumBetAmount = data.minimumBetAmount;

          document.getElementById("input-betting-amount").value = data.minimumBetAmount;

          var bettingSlider = document.getElementById("betting-amount-slider");
          bettingSlider.min = data.minimumBetAmount;
          bettingSlider.max = data.maximumBetAmount;
          bettingSlider.value = data.minimumBetAmount;
        }
        else {
          canBet = false;
          document.getElementById("bet-button").style.display = "none";
        }
        if (data.canRaise) {
          document.getElementById("betting-controls").style.display = "block";
          canRaise = true;
          document.getElementById("raise-button").style.display = "block";
          document.getElementById("raise-button").innerHTML = "Raise to " + data.minimumRaiseAmount;
          document.getElementById("raise-button").value = data.minimumRaiseAmount;

          console.log("raise interval: " + data.raiseInterval +  " pot size: " + data.potSize);
          document.getElementById("minus-bet-button").value = data.raiseInterval;
          document.getElementById("plus-bet-button").value = data.raiseInterval;
          var halfPotSizeRaise = data.wager + data.callAmount + 0.5 * (data.potSize + data.callAmount);
          if (halfPotSizeRaise > data.maximumRaiseAmount)
            halfPotSizeRaise = data.maximumRaiseAmount;
          document.getElementById("half-pot-bet-button").value = Math.round(halfPotSizeRaise);
          var potSizeRaise = data.wager + data.callAmount + (data.potSize + data.callAmount);
          if (potSizeRaise > data.maximumRaiseAmount)
            potSizeRaise = data.maximumRaiseAmount;
          document.getElementById("pot-bet-button").value = potSizeRaise;
          document.getElementById("max-bet-button").value = data.maximumRaiseAmount;
          maximumRaiseAmount = data.maximumRaiseAmount;
          minimumRaiseAmount = data.minimumRaiseAmount;

          document.getElementById("input-betting-amount").value = data.minimumRaiseAmount;

          var bettingSlider = document.getElementById("betting-amount-slider");
          bettingSlider.min = data.minimumRaiseAmount;
          bettingSlider.max = data.maximumRaiseAmount;
          bettingSlider.value = data.minimumRaiseAmount;
          if (minimumRaiseAmount == maximumRaiseAmount) {
            document.getElementById("betting-controls").style.display = "none";
          }
        }
        else {
          canRaise = false;
          document.getElementById("raise-button").style.display = "none";
        }
        if (!data.canBet && !data.canRaise) {
          document.getElementById("betting-controls").style.display = "none";
        }
        if (document.getElementById("action-view").style.display == "none") {
          document.getElementById("notification-badge").style.display = "block";
          document.getElementById("notification-badge-two").style.display = "block";
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
    document.getElementById("notification-badge").style.display = "none";
    document.getElementById("notification-badge-two").style.display = "none";
  });
  $("#hand-history-button").on("click", function(evt) {
    vm.show("hand-history-view");
  });
  $("#ranking-guide-button").on("click", function(evt) {
    vm.show("ranking-guide-view");
  });
  $("#special-row").on("click", function(evt) {
    evt.stopPropagation();
    evt.preventDefault();
    evt.stopImmediatePropagation();
    console.log("special row was clicked");
    var x = document.getElementById("hand-history-expanded");
    if (x.style.display === "none") {
        x.style.backgroundColor = "rgb(15,48,61)";
        x.style.display = "table-row";
    } else {
        x.style.backgroundColor = "rgb(116,203,234)";
        x.style.display = "none";
    }
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

  var cardsShowing = false;
  var canToggleCards = false;

  $("#card-row").on("click", function(evt) {
    evt.stopPropagation();
    evt.preventDefault();
    evt.stopImmediatePropagation();
    if (!canToggleCards)
      return;
    if (!shakeDisabled) {
      document.getElementById('hole-card-1').className -= "shake";
      document.getElementById('hole-card-2').className -= "shake";
    }
    if (cardsShowing) {
      document.getElementById('hole-card-1').style.backgroundImage = 'url(Cards/card_back.png)';
      document.getElementById('hole-card-2').style.backgroundImage = 'url(Cards/card_back.png)';
      cardsShowing = false;
    }
    else {
      document.getElementById('hole-card-1').style.backgroundImage = 'url(' + src1 + ')';
      document.getElementById('hole-card-2').style.backgroundImage = 'url(' + src2 + ')';
      cardsShowing = true;
    }
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
      else if (canRaise && newVal < minimumRaiseAmount)
        newVal = minimumRaiseAmount;
      if (canBet)
        updateBetAmount(newVal);
      else if (canRaise)
        updateRaiseAmount(newVal);
    }
    else if ($button.text() == "+") {
      var newVal = Number(button.value) + Number($button.val());
      if (canBet && newVal > maximumBetAmount)
        newVal = maximumBetAmount;
      else if (canRaise && newVal > maximumRaiseAmount)
        newVal = maximumRaiseAmount;
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
  if (canBet) {
    if (val < minimumBetAmount) {
      val = minimumBetAmount;
    }
    if (val > maximumBetAmount) {
      val = maximumBetAmount;
    }
    document.getElementById("bet-button").value = val;
    document.getElementById("bet-button").innerHTML = "Bet " + val;
  }
  else if (canRaise) {
    if (val < minimumRaiseAmount) {
      val = minimumRaiseAmount;
    }
    if (val > maximumRaiseAmount) {
      val = maximumRaiseAmount;
    }
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
  document.getElementById('hole-card-1').style.backgroundImage = "";
  document.getElementById('hole-card-2').style.backgroundImage = "";
  canToggleCards = false;

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
    return;
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

function popupText() {
  var popup = document.getElementById("myPopup");
  popup.classList.toggle("show");
}

function showHoleCard(id, src, width, height, alt) {
  var img = document.createElement("img");
  img.src = src;
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