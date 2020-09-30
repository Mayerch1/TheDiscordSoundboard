import asyncio
from quart import Quart, abort, request, make_response

from bot.bot import DiscordBot
import db.db_connector as db


# load sub-pages
from resources.bot_resource import bot_page
from resources.config_resource import config_page
from resources.button_resource import button_page
from resources.track_resource import track_page


SECRET_KEY = 'letmein'

app = Quart(__name__)

# NOTE: this has no cryptographical value, it should only prevent the avg pleb/crawler from scanning/modifying the db
app.config['SECRET_KEY'] = '6efb4653-a124-4941-af02-a26d4853aec7'

app.register_blueprint(bot_page)
app.register_blueprint(config_page)
app.register_blueprint(button_page)
app.register_blueprint(track_page)


    


@app.before_request
async def before_request():
    if not request.headers.get('SECRET_KEY') == app.config['SECRET_KEY']:
        abort(403)


@app.route("/api/version/latest")
async def get_latest_version():
    """get the latest version of the api
       the Location header will point to the latest endpoint (relative)

    Returns:
        [type]: 200
    """
    resp = await make_response('v1', 200)
    resp.headers['Location'] = '/api/v1/'
    return resp


@app.route("/api/v1")
def get_v1_version():
    """get a 200 response to validate that this version is still supported

    Returns:
        [type]: 200
    """
    return "v1", 200



def init_database():
    print('Initializing database...')
    db.SqlConnector.init_db()


init_database()
app.run()









