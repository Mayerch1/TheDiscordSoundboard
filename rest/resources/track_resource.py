import quart
from quart import jsonify, request, abort, make_response

from dao.buttonDao import TrackDataDao
from model.button import TrackData



track_page = quart.Blueprint('track_page', __name__, template_folder='templates')


@track_page.route('/api/v1/tracks') 
def get_all_tracks():
    
    
    track_list =  TrackDataDao.get_instance().get_track_list()

    # convert objects to json
    json_list = []
    [json_list.append(trk.to_json()) for trk in track_list]

    return jsonify(json_list)
    




@track_page.route('/api/v1/tracks/<id>') 
def get_track(id):

    if not id.isdigit():
        return abort(400)
    

    id = int(id)
    trk =  TrackDataDao.get_instance().get_track(id)

    if trk is not None:
        return jsonify(trk.to_json())
    else:
        return abort(404)


@track_page.route('/api/v1/tracks/', methods=['POST'])
async def add_track():
    body = await request.get_json()

    if body is None:
        abort(400)

    track = TrackData(body)
    track_inserted = TrackDataDao.get_instance().add_track(track)


    if track_inserted is None:
        abort(422)

    response = await make_response(jsonify(track_inserted.to_json()), 201)
    response.headers['Location'] = '/api/v1/tracks/' + str(track_inserted.id)

    return response


@track_page.route('/api/v1/tracks/<id>', methods=['DELETE'])
async def del_track(id):
    
    if not id.isdigit():
        return abort(400)


    id = int(id)

    dao = TrackDataDao.get_instance()

    if not dao.get_track_exists(id):
        abort(404)
    else:
        dao.delete_track(id)
        return 'Deleted', 200



@track_page.route('/api/v1/tracks/<id>', methods=['PUT'])
async def update_btn(id):

    body = await request.get_json()

    if not id.isdigit():
        abort(400)

    id = int(id)

    if body is None:
        abort(400)

    track = TrackData(body)
    track.id = id # override any given id with url path
    
    dao = TrackDataDao.get_instance()


    if not dao.get_track_exists(track.id):
        abort(404)
    else:
        dao.update_track(track)
        return 'Updated', 204