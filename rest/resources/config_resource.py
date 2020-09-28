import quart
from quart import jsonify, abort, request

from dao.configDao import ConfigDao
from model.config import Config

config_page = quart.Blueprint('config_page', __name__, template_folder='templates')


@config_page.route('/api/v1/config') 
def get_config_list():

    cfg_list = ConfigDao.get_instance().get_config_list()

    json_list = []
    [json_list.append(cfg.to_json()) for cfg in cfg_list]

    return jsonify(json_list), 200



@config_page.route('/api/v1/config/<id>') 
def get_config(id):

    if not id.isdigit():
        abort(400)

    id = int(id)
    cfg = ConfigDao.get_instance().get_config(id)

    if cfg is None:
        abort(404)
    
    return jsonify(cfg.to_json())





@config_page.route('/api/v1/config<id>', methods=['PUT'])
async def update_config(id):
    
    body = await request.get_jso()

    if not id.isdigit():
        abort(400)

    id = int(id)

    if body is None:
        abort(400)

    config = Config(body)
    config.id = 1 # only one config page at this time

    ConfigDao.get_instance().update_config(config)
    return 'Updated', 204