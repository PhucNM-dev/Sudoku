import 'devextreme/dist/css/dx.light.css';
import { createApp } from 'vue';
import App from './App.vue';
import DxDataGrid from 'devextreme-vue/data-grid';

const app = createApp(App);
app.component(DxDataGrid.name, DxDataGrid);
app.mount('#app');