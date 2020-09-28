import asyncio
from quart import Quart, abort, request

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


# custom discord wrapper class
# only here for DEBUG 
# will be moved to /bot endpoint
bot = DiscordBot()





@app.before_serving
async def before_serving():
    await bot.start_client_loop()
    

@app.before_request
async def before_request():
    if not request.headers.get('SECRET_KEY') == app.config['SECRET_KEY']:
        abort(403)


@app.route("/api/version/latest")
def get_latest_version():
    return "v1", 200


@app.route("/api/v1")
def get_v1_version():
    return "v1", 200



def init_database():
    print('Initializing database...')
    db.SqlConnector.init_db()


init_database()
app.run()









