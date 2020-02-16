class LogLevel {
    static ALL = 0;
    static DEBUG = 1;
    static INFO = 2;
    static ERROR = 3;
    static OFF = 4;
}

/**
 * A wrapper for the console.log that give level based control.
 * Use the instance constructor to set the level.
 * The default is ALL.
 */
class Logger {
    /**
     * Constructor
     * @param {LogLevel} level The lowest level of information to send to the console. The default is ALL.
     */
    constructor(level = 0) {
        this.logLevel = level;
        //this.prefix = "LOGGER." + Logger.levelToString(level) + "\n";
        //this.prefix = "Logger not initialized.";
        if (level < LogLevel.OFF) {
            console.log("Logger set to " + Logger.levelToString(level));
        }
    }

    prefix(level) {
        return "LOGGER." + Logger.levelToString(level) + "\n";
    }

    static levelToString(levelNumber) {
        switch (levelNumber) {
            case 0:
                return "ALL";
            case 1:
                return "DEBUG";
            case 2:
                return "INFO";
            case 3:
                return "ERROR";
            case 4:
                return "OFF";
            default:
                console.error("LogLevel.toString default case");
                break;
        }
    }

    all(msg) {
        this.log(msg, 0);
    }

    debug(msg) {
        this.log(msg, 1);
    }

    info(msg) {
        this.log(msg, 2);
    }

    error(msg) {
        this.log(msg, 3);
    }

    log(message, lvl = 1) {
        if (lvl >= this.logLevel && lvl <= LogLevel.INFO) {
            console.log(this.prefix(lvl) + message + "\n\n");
        } else if (lvl === LogLevel.ERROR) {
            console.error("LOGGER.ERROR\n" + message + "\n");
        } else {
            console.error("~!~!~!~!~!~ Logger Error ~!~!~!~!~!~\n");
        }
    }
}

/**
 * The instance of the Logger object for the project.
 * Change the LogLevel parameter in the constructor function
 * to change the level of logging in the application.
 */
let logger = new Logger(LogLevel.DEBUG);