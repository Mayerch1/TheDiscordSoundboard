import quart
from quart import jsonify, request, abort, make_response

from dao.buttonDao import ButtonDao
from model.button import Button



button_page = quart.Blueprint('button_page', __name__, template_folder='templates')


@button_page.route('/api/v1/buttons') 
def get_all_btns():
    
    
    btn_list =  ButtonDao.get_instance().get_button_list()

    # convert objects to json
    json_list = []
    [json_list.append(btn.to_json()) for btn in btn_list]

    return jsonify(json_list)
    


@button_page.route('/api/v1/buttons/<id>') 
def get_btn(id):

    if not id.isdigit():
        abort(400)
    

    id = int(id)
    btn =  ButtonDao.get_instance().get_button(id)

    if btn is None:
        abort(404)

    return jsonify(btn.to_json())



@button_page.route('/api/v1/buttons/', methods=['POST'])
async def add_btn():
    body = await request.get_json()

    if body is None:
        abort(400)

    btn = Button(body)
    btn_inserted = ButtonDao.get_instance().add_button(btn)


    if btn_inserted is None:
        abort(422)

    response = await make_response(jsonify(btn_inserted.to_json()), 201)
    response.headers['Location'] = '/api/v1/buttons/' + str(btn_inserted.id)
    
    return response



@button_page.route('/api/v1/buttons/<id>', methods=['DELETE'])
async def del_btn(id):
    
    if not id.isdigit():
        abort(400)


    id = int(id)

    dao = ButtonDao.get_instance()

    if not dao.get_button_exists(id):
        abort(404)
    else:
        dao.delete_button(id)
        return 'Deleted', 200



@button_page.route('/api/v1/buttons/<id>', methods=['PUT'])
async def update_btn(id):

    body = await request.get_json()

    if not id.isdigit():
        abort(400)

    id = int(id)

    if body is None:
        abort(400)

    button = Button(body)
    button.id = id # override any given id with url path
    dao = ButtonDao.get_instance()


    if not dao.get_button_exists(button.id):
        abort(404)
    else:
        dao.update_button(button)
        return 'Updated', 204