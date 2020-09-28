import quart


config_page = quart.Blueprint('config_page', __name__, template_folder='templates')


@config_page.route('/api/v1/config') 
def show():
    
    return "Ho", 200