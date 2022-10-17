import { File } from '@asyncapi/generator-react-sdk';
import { ArmTemplate } from '../components/ArmTemplate'

export default function ({ asyncapi, params }) {
    return <File name={'deploy.json'}>
        <ArmTemplate asyncapi={asyncapi} location={params.location}>
        </ArmTemplate>
    </File>;
}